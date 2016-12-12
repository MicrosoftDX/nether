using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            await AddNewPlayer(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), null,
                HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task As_an_admin_i_can_add_new_players()
        {
            _client = GetAdminClient();

            string gamerTag = Guid.NewGuid().ToString();
            var result = await AddNewPlayer(gamerTag, Guid.NewGuid().ToString(), null);

            Assert.Equal("/api/players/" + gamerTag, result.Item1.Headers.GetValues("Location").First());
            Assert.Equal($"{{\"gamerTag\":\"{gamerTag}\"}}", result.Item2);
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
            var groupResponse = await CreateGroup(g, HttpStatusCode.Created);
            Assert.Equal("/api/groups/" + groupName, groupResponse.Item1.Headers.GetValues("Location").First());
            Assert.Equal($"{{\"groupName\":\"{groupName}\"}}", groupResponse.Item2);

            //list groups and check the created group is in the list
            GroupListResponse allGroups = await GetAllGroups();
            Assert.True(allGroups.Groups.Any(gr => gr.Name == groupName));
        }

        [Fact]
        public async Task As_a_users_I_cannot_create_groups()
        {
            await CreateGroup(new GroupEntry
            {
                Name = "fail"
            }, HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task I_can_get_a_group_by_name()
        {
            _client = GetAdminClient();

            string name = Guid.NewGuid().ToString();

            await CreateGroup(new GroupEntry
            {
                Name = name
            });

            GroupGetResponse g = await GetGroupByName(name);

            Assert.Equal(name, g.Group.Name);
        }

        [Fact]
        public async Task Admin_can_update_groups()
        {
            _client = GetAdminClient();

            string name = Guid.NewGuid().ToString();

            await CreateGroup(new GroupEntry { Name = name, Description = "before change" });

            await UpdateGroup(new GroupEntry { Name = name, Description = "after change" });

            GroupGetResponse group = await GetGroupByName(name);

            Assert.Equal("after change", group.Group.Description);

        }

        #region [ REST helpers ]

        private async Task<Tuple<HttpResponseMessage, string>> CreateGroup(GroupEntry group, HttpStatusCode expectedCode = HttpStatusCode.Created)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync(BaseUri + "groups", group);

            Assert.Equal(expectedCode, response.StatusCode);

            string s = await response.Content.ReadAsStringAsync();

            return Tuple.Create(response, s);
        }

        private async Task UpdateGroup(GroupEntry group, HttpStatusCode expectedCode = HttpStatusCode.NoContent)
        {
            HttpResponseMessage response =  await _client.PutAsJsonAsync(BaseUri + "groups/" + group.Name, group);

            Assert.Equal(expectedCode, response.StatusCode);
        }

        private async Task<GroupListResponse> GetAllGroups(HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            return await HttpGet<GroupListResponse>(_client, BaseUri + "groups", expectedCode);
        }

        private async Task<GroupGetResponse> GetGroupByName(string name, HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            return await HttpGet<GroupGetResponse>(_client, BaseUri + "groups/" + name, expectedCode);
        }

        private async Task<PlayerListGetResponse> GetPlayersAsync(HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            return await HttpGet<PlayerListGetResponse>(_client, BaseUri + "players", expectedCode);
        }

        private async Task<PlayerGetResponse> GetPlayerAsync(HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            return await HttpGet<PlayerGetResponse>(_client, BaseUri + "player");
        }

        private async Task UpdatePlayerAsync(string country, HttpStatusCode expectedCode = HttpStatusCode.NoContent)
        {
            HttpResponseMessage response = await _client.PutAsJsonAsync(BaseUri + "player",
                new PlayerPostRequest
                {
                    Country = country,
                    CustomTag = null,
                    Gamertag = this.gamertag
                });
            Assert.Equal(expectedCode, response.StatusCode);
        }

        private async Task<Tuple<HttpResponseMessage, string>> AddNewPlayer(
            string gamerTag,
            string country,
            string customTag,
            HttpStatusCode expectedCode = HttpStatusCode.Created)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync(BaseUri + "players",
                new PlayerPostRequest
                {
                    Gamertag = gamerTag,
                    Country = country,
                    CustomTag = customTag
                });
            Assert.Equal(expectedCode, response.StatusCode);

            string s = await response.Content.ReadAsStringAsync();

            return Tuple.Create(response, s);
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

        public class PlayerEntry
        {
            public string Gamertag { get; set; }
            public string Country { get; set; }
            public string CustomTag { get; set; }
        }

        public class PlayerPostRequest
        {
            public string Gamertag { get; set; }
            public string Country { get; set; }
            public string CustomTag { get; set; }
        }

        public class GroupEntry
        {
            public string Name { get; set; }
            public string CustomType { get; set; }
            public string Description { get; set; }
        }

        #endregion
    }
}