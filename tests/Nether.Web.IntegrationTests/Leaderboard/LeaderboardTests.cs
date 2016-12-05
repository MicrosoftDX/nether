using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace Nether.Web.IntegrationTests.Leaderboard
{
    //todo: come up with better naming
    public class LeaderboardTests : WebTestBase
    {
        private const string BaseUri = "/api/leaderboard";
        private HttpClient _client;

        public LeaderboardTests()
        {
            _client = GetClient("testuser");
        }

        [Fact]
        public async Task Get_leaderboard_call_succeeds()
        {
            HttpResponseMessage response = await _client.GetAsync(BaseUri + "/default");
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Post_leaderboard_call_succeeds()
        {
            await PostScore(1);
        }

        [Fact]
        public async Task Posting_new_score_updates_leaderboard()
        {
            LeaderboardGetResponse leaderboardBefore = await GetLeaderboard();

            await PostScore(100);

            LeaderboardGetResponse leaderboardAfter = await GetLeaderboard();
            
        }

        [Fact]
        public async Task Posting_negative_score_causes_bad_request()
        {
            _client = GetClient("testuser");

            await PostScore(-5, HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Cannot_get_leaderboard_if_im_not_in_Player_role()
        {
            _client = GetClient("devadmin");    //login as devadmin who is not in "Player" role

            await GetLeaderboard("default", HttpStatusCode.Forbidden);
        }

        #region [ REST Wrappers ]

        private async Task<LeaderboardGetResponse> GetLeaderboard(string type = "default",
            HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            HttpResponseMessage response = await _client.GetAsync(BaseUri + "/" + type);
            Assert.Equal(expectedCode, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<LeaderboardGetResponse>(content);
        }

        private async Task PostScore(int score,
            HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync(BaseUri,
                new
                {
                    country = "UK",
                    score = score
                });

            Assert.Equal(expectedCode, response.StatusCode);
        }

        #endregion

        #region [ Model ]

        public class LeaderboardGetResponse
        {
            public LeaderboardEntry[] Entries { get; set; }

            public class LeaderboardEntry
            {
                public string Gamertag { get; set; }

                public int Score { get; set; }
            }
        }

        #endregion
    }
}