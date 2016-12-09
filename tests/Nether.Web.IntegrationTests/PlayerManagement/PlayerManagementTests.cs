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
        public async Task As_a_logged_in_player_i_can_get_my_info()
        {
            PlayerGetResponse myPlayer = await GetPlayerAsync();
        }

        #region [ REST helpers ]

        private async Task<PlayerGetResponse> GetPlayerAsync(HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            HttpResponseMessage response = await _client.GetAsync(BaseUri + "player");
            Assert.Equal(expectedCode, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PlayerGetResponse>(content);
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

        public class PlayerPostRequestModel
        {
            public string Gamertag { get; set; }
            public string Country { get; set; }
            public string CustomTag { get; set; }
        }

        #endregion
    }
}