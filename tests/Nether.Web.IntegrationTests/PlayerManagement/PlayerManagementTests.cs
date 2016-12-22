// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Nether.Web.IntegrationTests.PlayerManagement
{
    public class PlayerManagementTests : WebTestBase
    {
        private const string BaseUri = "/api/";
        private HttpClient _client;

        public PlayerManagementTests()
        {
            _client = GetClient();
        }

        [Fact]
        public async Task I_can_get_my_player_info()
        {
            PlayerGetResponse myPlayer = await GetPlayerAsync();
        }

        [Fact]
        public async Task I_can_update_my_info()
        {
            PlayerGetResponse beforeUpdate = await GetPlayerAsync();

            string newCountry = Guid.NewGuid().ToString();
            await UpdatePlayerAsync(newCountry);

            PlayerGetResponse afterUpdate = await GetPlayerAsync();
            Assert.Equal(newCountry, afterUpdate.Player.Country);
        }

        [Fact]
        public async Task As_a_user_i_cannot_add_new_players()
        {
            await AddNewPlayerAsync(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                null,
                HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task As_an_admin_i_can_add_new_players()
        {
            _client = GetAdminClient();

            string gamertag = Guid.NewGuid().ToString();
            string userId = Guid.NewGuid().ToString();
            var result = await AddNewPlayerAsync(
                                gamertag,
                                userId,
                                Guid.NewGuid().ToString(),
                                null);

            Assert.Equal("/api/players/" + gamertag, result.Response.Headers.GetValues("Location").First());
            Assert.Equal(gamertag, (string)result.ResponseBody.gamertag);

            //check that I can get player by gamertag
            PlayerGetResponse response = await GetPlayerAsync(gamertag);
        }

        [Fact]
        public async Task As_an_admin_i_can_get_all_players()
        {
            _client = GetAdminClient();

            PlayerListGetResponse response = await GetPlayersAsync();

            Assert.NotNull(response.Players);
            Assert.True(response.Players.Length > 0);
        }

        [Fact]
        public async Task As_a_normal_user_I_cant_get_player_list()
        {
            PlayerListGetResponse response = await GetPlayersAsync(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task As_an_admin_I_can_create_and_list_groups()
        {
            _client = GetAdminClient();

            string groupName = Guid.NewGuid().ToString();

            var g = new GroupEntry
            {
                Name = groupName,
                Description = "fake"
            };

            // validate group response
            var groupResponse = await CreateGroupAsync(g, HttpStatusCode.Created);
            Assert.Equal("/api/groups/" + groupName, groupResponse.Response.Headers.GetValues("Location").First());
            Assert.Equal(groupName, (string)groupResponse.ResponseBody.groupName);

            //list groups and check the created group is in the list
            GroupListResponse allGroups = await GetAllGroupsAsync();
            Assert.True(allGroups.Groups.Any(gr => gr.Name == groupName));
        }

        [Fact]
        public async Task As_a_users_I_can_create_and_list_my_groups()
        {
            _client = GetClient();

            string groupName = Guid.NewGuid().ToString();

            var g = new GroupEntry
            {
                Name = groupName,
                Description = "fake"
            };

            // validate group response
            var groupResponse = await CreateGroupAsync(g, HttpStatusCode.Created);
            Assert.Equal("/api/groups/" + groupName, groupResponse.Response.Headers.GetValues("Location").First());
            Assert.Equal(groupName, (string)groupResponse.ResponseBody.groupName);


            await AddPlayerToGroupAsync(groupName);


            //list groups and check the created group is in the list
            GroupListResponse allGroups = await GetPlayerGroupsAsync();
            Assert.True(allGroups.Groups.Any(gr => gr.Name == groupName));
        }

        [Fact]
        public async Task I_can_get_a_group_by_name()
        {
            _client = GetAdminClient();

            string name = Guid.NewGuid().ToString();

            await CreateGroupAsync(new GroupEntry
            {
                Name = name
            });

            GroupGetResponse g = await GetGroupByNameAsync(name);

            Assert.Equal(name, g.Group.Name);
        }

        [Fact]
        public async Task Admin_can_update_groups()
        {
            _client = GetAdminClient();

            string name = Guid.NewGuid().ToString();

            await CreateGroupAsync(new GroupEntry { Name = name, Description = "before change" });

            await UpdateGroupAsync(new GroupEntry { Name = name, Description = "after change" });

            GroupGetResponse group = await GetGroupByNameAsync(name);

            Assert.Equal("after change", group.Group.Description);
        }

        [Fact]
        public async Task Adding_and_removing_a_player_to_a_group()
        {
            _client = GetAdminClient();

            //create test group
            string groupName = Guid.NewGuid().ToString();
            await CreateGroupAsync(new GroupEntry { Name = groupName });

            //add myself to this group
            await AddPlayerToGroupAsync(groupName, _gamertag);

            //check that i'm in the group now
            GroupMembersResponseModel groupMembers = await GetGroupMembersAsync(groupName);
            Assert.True(groupMembers.Gamertags.Any(m => m == _gamertag));

            //remove me from this group
            await DeletePlayerFromGroupAsync(groupName, _gamertag);

            //check group has no gamertag of mine anymore
            groupMembers = await GetGroupMembersAsync(groupName);
            Assert.True(!groupMembers.Gamertags.Any(m => m == _gamertag));
        }

        [Fact]
        public async Task I_can_list_group_members()
        {
            _client = GetAdminClient();

            string groupName = Guid.NewGuid().ToString();
            await CreateGroupAsync(new GroupEntry { Name = groupName, Members = new[] { _gamertag, "testUserGamerTag" } });

            GroupMembersResponseModel group = await GetGroupMembersAsync(groupName);

            Assert.Equal(2, group.Gamertags.Length);
        }

        [Fact]
        public async Task I_can_find_out_which_groups_I_belong_to()
        {
            _client = GetClient();

            //first create two groups and add me to them
            string g1 = Guid.NewGuid().ToString();
            string g2 = Guid.NewGuid().ToString();
            await CreateGroupAsync(new GroupEntry { Name = g1, Members = new[] { _gamertag } });
            await CreateGroupAsync(new GroupEntry { Name = g2, Members = new[] { _gamertag } });

            //get my groups
            GroupListResponse groups = await GetPlayerGroupsAsync();
            Assert.True(groups.Groups.Any(g => g.Name == g1));
            Assert.True(groups.Groups.Any(g => g.Name == g2));
        }

        #region [ REST helpers ]

        private async Task<GroupListResponse> GetPlayerGroupsAsync(string gamerTag = null, HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            if (gamerTag == null)
            {
                return await HttpGet<GroupListResponse>(_client, BaseUri + "player/groups", expectedCode);
            }
            else
            {
                return await HttpGet<GroupListResponse>(_client, BaseUri + "players/" + gamerTag + "/groups", expectedCode);
            }
        }

        private async Task<GroupMembersResponseModel> GetGroupMembersAsync(string groupName, HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            return await HttpGet<GroupMembersResponseModel>(_client, $"{BaseUri}groups/{groupName}/players", expectedCode);
        }

        private async Task DeletePlayerFromGroupAsync(string groupName, string playerName, HttpStatusCode expectedCode = HttpStatusCode.NoContent)
        {
            HttpResponseMessage response = await _client.DeleteAsync($"{BaseUri}groups/{groupName}/players/{playerName}");

            Assert.Equal(expectedCode, response.StatusCode);
        }

        private async Task AddPlayerToGroupAsync(string groupName, string playerName = null, HttpStatusCode expetectedCode = HttpStatusCode.OK)
        {
            HttpResponseMessage response;

            if (playerName == null)
            {
                response = await _client.PutAsync($"{BaseUri}player/groups/{groupName}", null);
            }
            else
            {
                response = await _client.PutAsync($"{BaseUri}players/{playerName}/groups/{groupName}", null);
            }

            Assert.Equal(expetectedCode, response.StatusCode);
        }

        private async Task<ApiResponse> CreateGroupAsync(GroupEntry group, HttpStatusCode expectedCode = HttpStatusCode.Created)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync(BaseUri + "groups", group);

            Assert.Equal(expectedCode, response.StatusCode);

            dynamic r = await response.Content.ReadAsAsync<dynamic>();

            return new ApiResponse { Response = response, ResponseBody = r };
        }

        private async Task UpdateGroupAsync(GroupEntry group, HttpStatusCode expectedCode = HttpStatusCode.NoContent)
        {
            HttpResponseMessage response = await _client.PutAsJsonAsync(BaseUri + "groups/" + group.Name, group);

            Assert.Equal(expectedCode, response.StatusCode);
        }

        private async Task<GroupListResponse> GetAllGroupsAsync(HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            return await HttpGet<GroupListResponse>(_client, BaseUri + "groups", expectedCode);
        }

        private async Task<GroupGetResponse> GetGroupByNameAsync(string name, HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            return await HttpGet<GroupGetResponse>(_client, BaseUri + "groups/" + name, expectedCode);
        }

        private async Task<PlayerListGetResponse> GetPlayersAsync(HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            return await HttpGet<PlayerListGetResponse>(_client, BaseUri + "players", expectedCode);
        }

        private async Task<PlayerGetResponse> GetPlayerAsync(string gamerTag = null, HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            if (gamerTag == null)
            {
                return await HttpGet<PlayerGetResponse>(_client, BaseUri + "player");
            }

            return await HttpGet<PlayerGetResponse>(_client, BaseUri + "players/" + gamerTag);
        }

        private async Task UpdatePlayerAsync(string country, HttpStatusCode expectedCode = HttpStatusCode.NoContent)
        {
            HttpResponseMessage response = await _client.PutAsJsonAsync(BaseUri + "player",
                new
                {
                    Country = country,
                    CustomTag = (string)null,
                    Gamertag = _gamertag
                });
            Assert.Equal(expectedCode, response.StatusCode);
        }

        private async Task<ApiResponse> AddNewPlayerAsync(
            string gamerTag,
            string userId,
            string country,
            string customTag,
            HttpStatusCode expectedCode = HttpStatusCode.Created)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync(BaseUri + "players",
                new
                {
                    UserId = userId,
                    Gamertag = gamerTag,
                    Country = country,
                    CustomTag = customTag
                });
            Assert.Equal(expectedCode, response.StatusCode);

            dynamic r = await response.Content.ReadAsAsync<dynamic>();

            return new ApiResponse { Response = response, ResponseBody = r };
        }

        public class ApiResponse
        {
            public HttpResponseMessage Response { get; set; }
            public dynamic ResponseBody { get; set; }
        }

        public class GroupMembersResponseModel
        {
            public string[] Gamertags { get; set; }
        }

        public class PlayerListGetResponse
        {
            public PlayerEntry[] Players { get; set; }
        }

        public class GroupListResponse
        {
            public GroupEntry[] Groups { get; set; }
        }

        public class PlayerGetResponse
        {
            public PlayerEntry Player { get; set; }
        }

        public class GroupGetResponse
        {
            public GroupEntry Group { get; set; }
        }

        [DebuggerDisplay("Player: {Gamertag}")]
        public class PlayerEntry
        {
            public string Gamertag { get; set; }
            public string Country { get; set; }
            public string CustomTag { get; set; }
        }

        [DebuggerDisplay("Group: {Name}")]
        public class GroupEntry
        {
            public string Name { get; set; }
            public string CustomType { get; set; }
            public string Description { get; set; }
            public string[] Members { get; set; }
        }

        #endregion
    }
}