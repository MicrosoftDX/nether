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
    public class BasicTests : WebTestBase
    {
        private const string BaseUri = "/api/leaderboard";

        [Fact]
        public async Task Get_leaderboard_call_succeeds()
        {
            HttpResponseMessage response = await Client.GetAsync(BaseUri);
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Posting_new_score_updates_leaderboard()
        {
            LeaderboardGetResponse leaderboardBefore = await GetLeaderboard();

            await PostScore(Username, 100);

            LeaderboardGetResponse leaderboardAfter = await GetLeaderboard();
            
        }

        #region [ REST Wrappers ]

        private async Task<LeaderboardGetResponse> GetLeaderboard()
        {
            HttpResponseMessage response = await Client.GetAsync(BaseUri);
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<LeaderboardGetResponse>(content);
        }

        private async Task PostScore(string gamerTag, int score)
        {
            HttpResponseMessage response = await Client.PostAsJsonAsync(BaseUri,
                new
                {
                    country = "UK",
                    customTag = gamerTag,
                    score = score
                });

            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        #endregion

        #region [ Model ]

        public class LeaderboardGetResponse
        {
            public LeaderboardEntry[] LeaderboardEntries { get; set; }

            public class LeaderboardEntry
            {
                public string Gamertag { get; set; }

                public int Score { get; set; }
            }
        }

        #endregion
    }
}