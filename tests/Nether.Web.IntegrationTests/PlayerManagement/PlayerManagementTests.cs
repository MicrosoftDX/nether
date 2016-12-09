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

        #region [ REST helpers ]

        private async Task<PlayerGetResponse> GetPlayerAsync(HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            HttpResponseMessage response = await _client.GetAsync(BaseUri + "player");
            Assert.Equal(expectedCode, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PlayerGetResponse>(content);
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

        public class PlayerGetResponse
        {
            public PlayerEntry Player { get; set; }

            public class PlayerEntry
            {
                public string Gamertag { get; set; }
                public string Country { get; set; }
                public string CustomTag { get; set; }
            }
        }

        public class PlayerPostRequest
        {
            public string Gamertag { get; set; }
            public string Country { get; set; }
            public string CustomTag { get; set; }
        }

        #endregion
    }
}