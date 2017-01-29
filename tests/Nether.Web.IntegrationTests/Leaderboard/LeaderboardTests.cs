// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
    public class LeaderboardTests : WebTestBase, IClassFixture<IntegrationTestUsersFixture>
    {
        private const string BasePath = "/api/leaderboard";

        public LeaderboardTests()
        {
        }

        [Fact]
        public async Task Get_leaderboard_call_succeeds()
        {
            var client = await GetClientAsync();
            HttpResponseMessage response = await client.GetAsync(BasePath + "/Default");
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Post_leaderboard_call_succeeds()
        {
            var client = await GetClientAsync();
            await PostScoreAsync(client, 1);
        }

        [Fact]
        public async Task Posting_new_score_updates_default_leaderboard()
        {
            var client = await GetClientAsync(username: "testuser");

            LeaderboardGetResponse leaderboardBefore = await GetLeaderboardAsync(client);

            List<LeaderboardGetResponse.LeaderboardEntry> entries =
                leaderboardBefore.Entries.Where(e => e.Gamertag == "testuser").ToList();

            //check that there is only one or less (if score wasn't posted yet) entry per user
            Assert.True(entries.Count <= 1);
            int oldScore = entries.Count == 0 ? 0 : entries.First().Score;

            //update the score posting a different (higher) result
            int newScore = oldScore + 1;
            await PostScoreAsync(client, newScore);

            //check that leaderboard has the updated score
            LeaderboardGetResponse leaderboardAfter = await GetLeaderboardAsync(client);
            int newFreshScore = leaderboardAfter.Entries.Where(e => e.Gamertag == "testuser").Select(e => e.Score).First();
            Assert.Equal(newFreshScore, newScore);
        }

        [Fact]
        public async Task Posting_similar_score_gets_around_me()
        {
            const string leaderboardName = "5-AroundMe";

            // note: this test assumes that radius is set to 5
            // meaning you get 5 players above and 5 below (11 elements in response in general)

            // set up scores
            var scores = new[] {
                    new { username = "testuser1", score = 100 },
                    new { username = "testuser2", score = 200 },
                    new { username = "testuser3", score = 300 },
                    new { username = "testuser4", score = 400 },
                    new { username = "testuser5", score = 500 },
                    new { username = "testuser6", score = 600 },
                    new { username = "testuser7", score = 700 },
                    new { username = "testuser8", score = 800 },
                    new { username = "testuser9", score = 900 },
                    new { username = "testuser10", score = 1000 },
                    new { username = "testuser11", score = 1100 },
                    new { username = "testuser12", score = 1200 },
                };
            foreach (var score in scores)
            {
                var tempClient = await GetClientAsync(score.username);
                await DeleteMyScoresAsync(tempClient);
                await PostScoreAsync(tempClient, score.score);
            }


            var client = await GetClientAsync(username: "testuser");
            await DeleteMyScoresAsync(client);
            await PostScoreAsync(client, 750);

            var response = await GetLeaderboardAsync(client, leaderboardName);
            Assert.Collection(response.Entries, 
                entry => Assert.Equal("testuser12", entry.Gamertag),
                entry => Assert.Equal("testuser11", entry.Gamertag),
                entry => Assert.Equal("testuser10", entry.Gamertag),
                entry => Assert.Equal("testuser9", entry.Gamertag),
                entry => Assert.Equal("testuser8", entry.Gamertag),
                entry => Assert.Equal("testuser", entry.Gamertag),
                entry => Assert.Equal("testuser7", entry.Gamertag),
                entry => Assert.Equal("testuser6", entry.Gamertag),
                entry => Assert.Equal("testuser5", entry.Gamertag),
                entry => Assert.Equal("testuser4", entry.Gamertag),
                entry => Assert.Equal("testuser3", entry.Gamertag)
             );

            // update testuser make their score similar to mine and check they are around me
            await PostScoreAsync(client, 1050);
            response = await GetLeaderboardAsync(client, leaderboardName);
            Assert.Collection(response.Entries,
                entry => Assert.Equal("testuser12", entry.Gamertag),
                entry => Assert.Equal("testuser11", entry.Gamertag),
                entry => Assert.Equal("testuser", entry.Gamertag),
                entry => Assert.Equal("testuser10", entry.Gamertag),
                entry => Assert.Equal("testuser9", entry.Gamertag),
                entry => Assert.Equal("testuser8", entry.Gamertag),
                entry => Assert.Equal("testuser7", entry.Gamertag),
                entry => Assert.Equal("testuser6", entry.Gamertag)
             );
        }

        [Fact]
        public async Task Limiting_top_scores_returns_limited_numer_of_rows()
        {
            var client = await GetClientAsync();
            LeaderboardGetResponse response = await GetLeaderboardAsync(client, "Top-5", HttpStatusCode.OK);

            Assert.True(response.Entries.Length <= 5);
        }

        [Fact]
        public async Task Posting_negative_score_causes_bad_request()
        {
            var client = await GetClientAsync("testuser");

            await PostScoreAsync(client, -5, HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Cannot_get_leaderboard_if_im_not_in_Player_role()
        {
            var client = await GetAdminClientAsync();    //login as devadmin who is not in "Player" role

            await GetLeaderboardAsync(client, "Default", HttpStatusCode.Forbidden);
        }

        #region [ REST Wrappers ]

        private async Task DeleteMyScoresAsync(HttpClient client)
        {
            await client.DeleteAsync(BasePath);
        }

        private async Task<LeaderboardGetResponse> GetLeaderboardAsync(
            HttpClient client,
            string type = "Default",
            HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            HttpResponseMessage response = await client.GetAsync(BasePath + "/" + type);
            Assert.Equal(expectedCode, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<LeaderboardGetResponse>(content);
        }

        private async Task PostScoreAsync(
            HttpClient client,
            int score,
            HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(BasePath,
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

                public override string ToString()
                {
                    return $"{Gamertag}\t {Score}";
                }
            }
        }

        #endregion
    }
}