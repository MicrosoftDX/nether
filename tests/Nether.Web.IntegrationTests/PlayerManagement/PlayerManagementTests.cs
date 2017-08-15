
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

using Nether.Web.IntegrationTests;
using System.Net.Http.Headers;
using System.Text;

namespace Nether.Web.IntegrationTests.PlayerManagement
{
    public class PlayerManagementTests : WebTestBase, IClassFixture<IntegrationTestUsersFixture>
    {
        private const string BasePath = "/api/";


        [Fact]
        public async Task As_a_player_I_can_get_my_player_info()
        {
            var client = await SignInAsync();

            PlayerGetResponse myPlayer = await GetPlayerAsync(client);
        }

        [Fact]
        public async Task As_a_player_I_can_update_my_info()
        {
            var client = await SignInAsync(username: "testuser");
            PlayerGetResponse beforeUpdate = await GetPlayerAsync(client);

            string newCountry = Guid.NewGuid().ToString();
            await UpdatePlayerAsync(client, newCountry, gamertag: "testuser");

            PlayerGetResponse afterUpdate = await GetPlayerAsync(client);
            Assert.Equal(newCountry, afterUpdate.Player.Country);
        }

        [Fact]
        public async Task As_a_player_I_can_update_my_player_state()
        {
            var client = await SignInAsync(username: "testuser-state");
            var newState = Guid.NewGuid().ToString();

            // Update state
            var response = await client.PutAsync($"{BasePath}player/state", new StringContent("SomeNewState", Encoding.UTF8, "text/plain"));
            await response.AssertStatusCodeAsync(HttpStatusCode.OK);

            // Read state
            response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, $"{BasePath}player/state")
            {
                Headers =
                {
                    Accept =
                    {
                        new MediaTypeWithQualityHeaderValue("text/plain")
                    }
                }
            });
            await response.AssertStatusCodeAsync(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal("SomeNewState", content);
        }

        [Fact]
        public async Task As_an_admin_I_can_update_player_state()
        {
            var client = await SignInAsAdminAsync();
            var newState = Guid.NewGuid().ToString();

            // Update state
            var response = await client.PutAsync($"{BasePath}admin/players/testuser-state/state", new StringContent("SomeNewState", Encoding.UTF8, "text/plain"));
            await response.AssertStatusCodeAsync(HttpStatusCode.OK);

            // Read state
            response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, $"{BasePath}admin/players/testuser-state/state")
            {
                Headers =
                {
                    Accept =
                    {
                        new MediaTypeWithQualityHeaderValue("text/plain")
                    }
                }
            });
            await response.AssertStatusCodeAsync(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal("SomeNewState", content);
        }

        [Fact]
        public async Task As_a_player_I_cannot_add_new_players()
        {
            var client = await SignInAsync();
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
            var client = await SignInAsAdminAsync();

            string gamertag = Guid.NewGuid().ToString();
            string userId = Guid.NewGuid().ToString();
            var result = await AddNewPlayerAsync(
                                client,
                                gamertag,
                                userId,
                                Guid.NewGuid().ToString(),
                                "IntegrationTestUser");

            Assert.Equal($"{BaseUrl}api/admin/players/" + gamertag, result.Response.Headers.GetValues("Location").First());

            //check that I can get player by gamertag
            PlayerGetResponse response = await GetPlayerAsync(client, gamertag);
        }

        [Fact]
        public async Task As_an_admin_I_can_get_all_players()
        {
            var client = await SignInAsAdminAsync();

            PlayerListGetResponse response = await GetPlayersAdminAsync(client);

            Assert.NotNull(response.Players);
            Assert.True(response.Players.Length > 0);
        }

        [Fact]
        public async Task As_a_player_user_I_cant_get_player_list()
        {
            var client = await SignInAsync();

            PlayerListGetResponse response = await GetPlayersAdminAsync(client, HttpStatusCode.Forbidden);
        }

        #region [ REST helpers ]
        private async Task<PlayerListGetResponse> GetPlayersAdminAsync(HttpClient client, HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            return await HttpGet<PlayerListGetResponse>(client, BasePath + "admin/players", expectedCode);
        }

        private async Task<PlayerGetResponse> GetPlayerAsync(HttpClient client, string gamerTag = null, HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            if (gamerTag == null)
            {
                return await HttpGet<PlayerGetResponse>(client, BasePath + "player");
            }

            return await HttpGet<PlayerGetResponse>(client, BasePath + "admin/players/" + gamerTag);
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
            HttpResponseMessage response = await client.PostAsJsonAsync(BasePath + "admin/players",
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



        public class PlayerGetResponse
        {
            public PlayerEntry Player { get; set; }
        }


        [DebuggerDisplay("Player: {Gamertag}")]
        public class PlayerEntry
        {
            public string Gamertag { get; set; }
            public string Country { get; set; }
            public string CustomTag { get; set; }
        }

        #endregion
    }
}