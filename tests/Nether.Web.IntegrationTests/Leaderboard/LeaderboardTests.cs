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

        [Fact]
        public async Task Get_leaderboard_call_succeeds()
        {
            HttpResponseMessage response = await Client.GetAsync(BaseUri + "/default");
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
            await PostScore(-5, HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Cannot_post_score_if_im_not_in_Player_role()
        {
            await PostScore(50, HttpStatusCode.Forbidden);
        }

        #region [ REST Wrappers ]

        private async Task<LeaderboardGetResponse> GetLeaderboard(string type = "default")
        {
            HttpResponseMessage response = await Client.GetAsync(BaseUri + "/" + type);
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<LeaderboardGetResponse>(content);
        }

        private async Task PostScore(int score,
            HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            HttpResponseMessage response = await Client.PostAsJsonAsync(BaseUri,
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