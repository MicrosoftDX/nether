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
        private const string LeaderboardBasePath = "/api/leaderboards";
        private const string ScoresBasePath = "/api/scores";

        public LeaderboardTests()
        {
        }

        [Fact]
        public async Task Get_leaderboard_call_succeeds()
        {
            var client = await SignInAsync();
            HttpResponseMessage response = await client.GetAsync(LeaderboardBasePath + "/Default");
            await response.AssertStatusCodeAsync(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Post_leaderboard_call_succeeds()
        {
            var client = await SignInAsync();
            await PostScoreAsync(client, 1);
        }

        [Fact]
        public async Task Posting_new_score_updates_default_leaderboard()
        {
            var client = await SignInAsync(username: "testuser");

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
            const string leaderboardName = "5_AroundMe";

            // note: this test assumes that radius is set to 5
            // meaning you get 5 players above and 5 below (11 elements in response in general)

            // set up scores
            var scores = new[] {
                    new { username = "testuser1", score = 1000 },
                    new { username = "testuser2", score = 2000 },
                    new { username = "testuser3", score = 3000 },
                    new { username = "testuser4", score = 4000 },
                    new { username = "testuser5", score = 5000 },
                    new { username = "testuser6", score = 6000 },
                    new { username = "testuser7", score = 7000 },
                    new { username = "testuser8", score = 8000 },
                    new { username = "testuser9", score = 9000 },
                    new { username = "testuser10", score = 10000 },
                    new { username = "testuser11", score = 11000 },
                    new { username = "testuser12", score = 12000 },
                };
            foreach (var score in scores)
            {
                var tempClient = await SignInAsync(score.username);
                await DeleteMyScoresAsync(tempClient);
                await PostScoreAsync(tempClient, score.score);
            }


            var client = await SignInAsync(username: "testuser");
            await DeleteMyScoresAsync(client);
            await PostScoreAsync(client, 7500);

            var response = await GetLeaderboardAsync(client, leaderboardName);
            Assert.Collection(response.Entries,
                entry => { Assert.Equal("testuser12", entry.Gamertag); Assert.Equal(1, entry.Rank); },
                entry => { Assert.Equal("testuser11", entry.Gamertag); Assert.Equal(2, entry.Rank); },
                entry => { Assert.Equal("testuser10", entry.Gamertag); Assert.Equal(3, entry.Rank); },
                entry => { Assert.Equal("testuser9", entry.Gamertag); Assert.Equal(4, entry.Rank); },
                entry => { Assert.Equal("testuser8", entry.Gamertag); Assert.Equal(5, entry.Rank); },
                entry => { Assert.Equal("testuser", entry.Gamertag); Assert.Equal(6, entry.Rank); },
                entry => { Assert.Equal("testuser7", entry.Gamertag); Assert.Equal(7, entry.Rank); },
                entry => { Assert.Equal("testuser6", entry.Gamertag); Assert.Equal(8, entry.Rank); },
                entry => { Assert.Equal("testuser5", entry.Gamertag); Assert.Equal(9, entry.Rank); },
                entry => { Assert.Equal("testuser4", entry.Gamertag); Assert.Equal(10, entry.Rank); },
                entry => { Assert.Equal("testuser3", entry.Gamertag); Assert.Equal(11, entry.Rank); }
             );


            // update score to one that matches another score
            await PostScoreAsync(client, 9000);
            response = await GetLeaderboardAsync(client, leaderboardName);
            Assert.Collection(response.Entries,
                entry => { Assert.Equal("testuser12", entry.Gamertag); Assert.Equal(1, entry.Rank); },
                entry => { Assert.Equal("testuser11", entry.Gamertag); Assert.Equal(2, entry.Rank); },
                entry => { Assert.Equal("testuser10", entry.Gamertag); Assert.Equal(3, entry.Rank); },
                entry => { Assert.Equal("testuser", entry.Gamertag); Assert.Equal(4, entry.Rank); }, // note equal to rank below
                entry => { Assert.Equal("testuser9", entry.Gamertag); Assert.Equal(4, entry.Rank); },
                entry => { Assert.Equal("testuser8", entry.Gamertag); Assert.Equal(6, entry.Rank); },
                entry => { Assert.Equal("testuser7", entry.Gamertag); Assert.Equal(7, entry.Rank); },
                entry => { Assert.Equal("testuser6", entry.Gamertag); Assert.Equal(8, entry.Rank); },
                entry => { Assert.Equal("testuser5", entry.Gamertag); Assert.Equal(9, entry.Rank); }
                // TODO - The current implementation filters rows based on rank...
                // Should we change it to be smarter and return consistent numbers of row regardless of duplicate scores?
                //entry => { Assert.Equal("testuser4", entry.Gamertag); Assert.Equal(10, entry.Rank); },
                //entry => { Assert.Equal("testuser3", entry.Gamertag); Assert.Equal(11, entry.Rank); }
             );
            // update testuser make their score similar to mine and check they are around me
            await PostScoreAsync(client, 10500);
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
            var client = await SignInAsync();
            LeaderboardGetResponse response = await GetLeaderboardAsync(client, "Top_5", HttpStatusCode.OK);

            Assert.True(response.Entries.Length <= 5);
        }

        [Fact]
        public async Task Posting_negative_score_causes_bad_request()
        {
            var client = await SignInAsync("testuser");

            await PostScoreAsync(client, -5, HttpStatusCode.BadRequest);
        }

        #region [ REST Wrappers ]

        private async Task DeleteMyScoresAsync(HttpClient client)
        {
            var response = await client.DeleteAsync(ScoresBasePath);
            await response.AssertSuccessStatusCodeAsync();
        }

        private async Task<LeaderboardGetResponse> GetLeaderboardAsync(
            HttpClient client,
            string type = "Default",
            HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            HttpResponseMessage response = await client.GetAsync(LeaderboardBasePath + "/" + type);
            await response.AssertStatusCodeAsync(expectedCode);

            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<LeaderboardGetResponse>(content);
        }

        private async Task PostScoreAsync(
            HttpClient client,
            int score,
            HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(ScoresBasePath,
                new
                {
                    country = "UK",
                    score = score
                });

            await response.AssertStatusCodeAsync(expectedCode);
        }

        #endregion

        #region [ Model ]

        public class LeaderboardGetResponse
        {
            public LeaderboardEntry[] Entries { get; set; }

            public class LeaderboardEntry
            {
                public int Rank { get; set; }

                public string Gamertag { get; set; }

                public int Score { get; set; }

                public override string ToString()
                {
                    return $"{Rank}: {Gamertag}\t {Score}";
                }
            }
        }

        #endregion
    }
}