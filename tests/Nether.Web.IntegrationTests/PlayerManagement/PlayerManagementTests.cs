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
    public class PlayerManagementTests : WebTestBase, IClassFixture<IntegrationTestUsersFixture>
    {
        private const string BasePath = "/api/";


        [Fact]
        public async Task As_a_player_I_can_get_my_player_info()
        {
            var client = await GetClientAsync();

            PlayerGetResponse myPlayer = await GetPlayerAsync(client);
        }

        [Fact]
        public async Task As_a_player_I_can_update_my_info()
        {
            var client = await GetClientAsync(username: "testuser");
            PlayerGetResponse beforeUpdate = await GetPlayerAsync(client);

            string newCountry = Guid.NewGuid().ToString();
            await UpdatePlayerAsync(client, newCountry, gamertag: "testuser");

            PlayerGetResponse afterUpdate = await GetPlayerAsync(client);
            Assert.Equal(newCountry, afterUpdate.Player.Country);
        }

        [Fact]
        public async Task As_a_player_I_cannot_add_new_players()
        {
            var client = await GetClientAsync();
            await AddNewPlayerAsync(
                client,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                "IntegrationTestUser",
                HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task As_an_admin_I_can_add_new_players()
        {
            var client = await GetAdminClientAsync();

            string gamertag = Guid.NewGuid().ToString();
            string userId = Guid.NewGuid().ToString();
            var result = await AddNewPlayerAsync(
                                client,
                                gamertag,
                                userId,
                                Guid.NewGuid().ToString(),
                                "IntegrationTestUser");

            Assert.Equal($"{BaseUrl}api/players/" + gamertag, result.Response.Headers.GetValues("Location").First());

            //check that I can get player by gamertag
            PlayerGetResponse response = await GetPlayerAsync(client, gamertag);
        }

        [Fact]
        public async Task As_an_admin_I_can_get_all_players()
        {
            var client = await GetAdminClientAsync();

            PlayerListGetResponse response = await GetPlayersAsync(client);

            Assert.NotNull(response.Players);
            Assert.True(response.Players.Length > 0);
        }

        [Fact]
        public async Task As_a_player_user_I_cant_get_player_list()
        {
            var client = await GetClientAsync();

            PlayerListGetResponse response = await GetPlayersAsync(client, HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task As_an_admin_I_can_create_and_list_groups()
        {
            var client = await GetAdminClientAsync();

            string groupName = Guid.NewGuid().ToString();

            var group = new GroupEntry
            {
                Name = groupName,
                Description = "fake"
            };

            // validate group response
            var groupResponse = await CreateGroupAsync(client, group, HttpStatusCode.Created);
            Assert.Equal($"{BaseUrl}api/groups/" + groupName, groupResponse.Response.Headers.GetValues("Location").First());

            //list groups and check the created group is in the list
            GroupListResponse allGroups = await GetAllGroupsAsync(client);
            Assert.True(allGroups.Groups.Any(g => g.Name == groupName));
        }

        [Fact]
        public async Task As_a_player_I_can_create_and_list_my_groups()
        {
            var client = await GetClientAsync();

            string groupName = Guid.NewGuid().ToString();

            var group = new GroupEntry
            {
                Name = groupName,
                Description = "fake"
            };

            // validate group response
            var groupResponse = await CreateGroupAsync(client, group, HttpStatusCode.Created);
            Assert.Equal($"{BaseUrl}api/groups/" + groupName, groupResponse.Response.Headers.GetValues("Location").First());

            await AddPlayerToGroupAsync(client, groupName);


            //list groups and check the created group is in the list
            GroupListResponse allGroups = await GetPlayerGroupsAsync(client);
            Assert.True(allGroups.Groups.Any(gr => gr.Name == groupName));
        }

        [Fact]
        public async Task As_an_admin_I_can_get_a_group_by_name()
        {
            var client = await GetAdminClientAsync();

            string name = Guid.NewGuid().ToString();

            await CreateGroupAsync(client, new GroupEntry
            {
                Name = name
            });

            GroupGetResponse g = await GetGroupByNameAsync(client, name);

            Assert.Equal(name, g.Group.Name);
        }

        [Fact]
        public async Task As_an_admin_I_can_update_groups()
        {
            var client = await GetAdminClientAsync();

            string name = Guid.NewGuid().ToString();

            await CreateGroupAsync(client, new GroupEntry { Name = name, Description = "before change" });

            await UpdateGroupAsync(client, new GroupEntry { Name = name, Description = "after change" });

            GroupGetResponse group = await GetGroupByNameAsync(client, name);

            Assert.Equal("after change", group.Group.Description);
        }

        [Fact]
        public async Task As_an_admin_I_can_add_and_remove_a_player_to_a_group()
        {
            var client = await GetClientAsync(username: "testuser"); // ensure player is created
            client = await GetAdminClientAsync();

            //create test group
            string groupName = Guid.NewGuid().ToString();
            await CreateGroupAsync(client, new GroupEntry { Name = groupName });

            // TODO - clean this test up so that it's not relying on _gamertag shared state

            //add myself to this group
            await AddPlayerToGroupAsync(client, groupName, "testuser");

            //check that i'm in the group now
            GroupMembersResponseModel groupMembers = await GetGroupMembersAsync(client, groupName);
            Assert.True(groupMembers.Gamertags.Any(m => m == "testuser"));

            //remove me from this group
            await DeletePlayerFromGroupAsync(client, groupName, "testuser");

            //check group has no gamertag of mine anymore
            groupMembers = await GetGroupMembersAsync(client, groupName);
            Assert.True(!groupMembers.Gamertags.Any(m => m == "testuser"));
        }

        [Fact]
        public async Task As_a_player_I_can_list_group_members()
        {
            var client = await GetClientAsync(username: "testuser");

            string groupName = Guid.NewGuid().ToString();
            await CreateGroupAsync(client, new GroupEntry { Name = groupName, Members = new[] { "testuser" } });

            GroupMembersResponseModel group = await GetGroupMembersAsync(client, groupName);

            Assert.Equal(1, group.Gamertags.Length);
        }

        [Fact]
        public async Task As_a_player_I_can_find_out_which_groups_I_belong_to()
        {
            var client = await GetClientAsync(username: "testuser");

            //first create two groups and add me to them
            string g1 = Guid.NewGuid().ToString();
            string g2 = Guid.NewGuid().ToString();
            await CreateGroupAsync(client, new GroupEntry { Name = g1, Members = new[] { "testuser" } });
            await CreateGroupAsync(client, new GroupEntry { Name = g2, Members = new[] { "testuser" } });

            //get my groups
            GroupListResponse groups = await GetPlayerGroupsAsync(client);
            Assert.True(groups.Groups.Any(g => g.Name == g1));
            Assert.True(groups.Groups.Any(g => g.Name == g2));
        }

        #region [ REST helpers ]

        private async Task<GroupListResponse> GetPlayerGroupsAsync(HttpClient client, string gamerTag = null, HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            if (gamerTag == null)
            {
                return await HttpGet<GroupListResponse>(client, BasePath + "player/groups", expectedCode);
            }
            else
            {
                return await HttpGet<GroupListResponse>(client, BasePath + "players/" + gamerTag + "/groups", expectedCode);
            }
        }

        private async Task<GroupMembersResponseModel> GetGroupMembersAsync(HttpClient client, string groupName, HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            return await HttpGet<GroupMembersResponseModel>(client, $"{BasePath}groups/{groupName}/players", expectedCode);
        }

        private async Task DeletePlayerFromGroupAsync(HttpClient client, string groupName, string playerName, HttpStatusCode expectedCode = HttpStatusCode.NoContent)
        {
            HttpResponseMessage response = await client.DeleteAsync($"{BasePath}groups/{groupName}/players/{playerName}");

            Assert.Equal(expectedCode, response.StatusCode);
        }

        private async Task AddPlayerToGroupAsync(HttpClient client, string groupName, string playerName = null, HttpStatusCode expectedStatus = HttpStatusCode.NoContent)
        {
            HttpResponseMessage response;

            if (playerName == null)
            {
                response = await client.PutAsync($"{BasePath}player/groups/{groupName}", null);
            }
            else
            {
                response = await client.PutAsync($"{BasePath}players/{playerName}/groups/{groupName}", null);
            }

            Assert.Equal(expectedStatus, response.StatusCode);
        }

        private async Task<ApiResponse> CreateGroupAsync(HttpClient client, GroupEntry group, HttpStatusCode expectedCode = HttpStatusCode.Created)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(BasePath + "groups", group);

            Assert.Equal(expectedCode, response.StatusCode);

            dynamic r = await response.Content.ReadAsAsync<dynamic>();

            return new ApiResponse { Response = response, ResponseBody = r };
        }

        private async Task UpdateGroupAsync(HttpClient client, GroupEntry group, HttpStatusCode expectedCode = HttpStatusCode.NoContent)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(BasePath + "groups/" + group.Name, group);

            Assert.Equal(expectedCode, response.StatusCode);
        }

        private async Task<GroupListResponse> GetAllGroupsAsync(HttpClient client, HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            return await HttpGet<GroupListResponse>(client, BasePath + "groups", expectedCode);
        }

        private async Task<GroupGetResponse> GetGroupByNameAsync(HttpClient client, string name, HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            return await HttpGet<GroupGetResponse>(client, BasePath + "groups/" + name, expectedCode);
        }

        private async Task<PlayerListGetResponse> GetPlayersAsync(HttpClient client, HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            return await HttpGet<PlayerListGetResponse>(client, BasePath + "players", expectedCode);
        }

        private async Task<PlayerGetResponse> GetPlayerAsync(HttpClient client, string gamerTag = null, HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            if (gamerTag == null)
            {
                return await HttpGet<PlayerGetResponse>(client, BasePath + "player");
            }

            return await HttpGet<PlayerGetResponse>(client, BasePath + "players/" + gamerTag);
        }

        private async Task UpdatePlayerAsync(HttpClient client, string country, string gamertag, HttpStatusCode expectedCode = HttpStatusCode.NoContent)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(BasePath + "player",
                new
                {
                    Country = country,
                    CustomTag = (string)null,
                    Gamertag = gamertag
                });
            Assert.Equal(expectedCode, response.StatusCode);
        }

        private async Task<ApiResponse> AddNewPlayerAsync(
            HttpClient client,
            string gamerTag,
            string userId,
            string country,
            string customTag,
            HttpStatusCode expectedCode = HttpStatusCode.Created)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(BasePath + "players",
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