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
    public class LeaderboardTests : WebTestBase
    {
        private const string BaseUri = "/api/leaderboard";

        public LeaderboardTests()
        {
        }

        [Fact]
        public async Task Get_leaderboard_call_succeeds()
        {
            var client = await GetClientAsync();
            HttpResponseMessage response = await client.GetAsync(BaseUri + "/Default");
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
            var client = await GetClientAsync();

            LeaderboardGetResponse leaderboardBefore = await GetLeaderboardAsync(client);

            List<LeaderboardGetResponse.LeaderboardEntry> entries =
                leaderboardBefore.Entries.Where(e => e.Gamertag == _gamertag).ToList();

            //check that there is only one or less (if score wasn't posted yet) entry per user
            Assert.True(entries.Count <= 1);
            int oldScore = entries.Count == 0 ? 0 : entries.First().Score;

            //update the score posting a different (higher) result
            int newScore = oldScore + 1;
            await PostScoreAsync(client, newScore);

            //check that leaderboard has the updated score
            LeaderboardGetResponse leaderboardAfter = await GetLeaderboardAsync(client);
            int newFreshScore = leaderboardAfter.Entries.Where(e => e.Gamertag == _gamertag).Select(e => e.Score).First();
            Assert.Equal(newFreshScore, newScore);
        }

        [Fact]
        public async Task Posting_similar_score_gets_around_me()
        {
            var client = await GetClientAsync();

            //note: radius is 2 at the moment, meaning you get 2 players above and 2 below (4 elements in response in general)

            //check there are at least 5 users
            LeaderboardGetResponse response = await GetLeaderboardAsync(client);
            if (response.Entries.Length < 5)
            {
                throw new NotImplementedException();    //todo: post scores to get at least 5
            }

            //todo: delete score entries before testing, this requires a separate http method

            //put me somewhere in the middle and push the other user in the bottom so they are not around me
            await DeleteMyScores(client);
            await PostScoreAsync(client, int.MaxValue / 2);
            string myGamertag = _gamertag;
            client = await GetClientAsync("testuser1");
            string theirGamertag = _gamertag;
            await DeleteMyScores(client);
            await PostScoreAsync(client, 1);

            //check they are not around me
            client = await GetClientAsync();
            response = await GetLeaderboardAsync(client, "5-AroundMe");
            Assert.False(response.Entries.Any(e => e.Gamertag == theirGamertag));

            //make their score similar to mine and check they are around me
            client = await GetClientAsync("testuser1");
            await PostScoreAsync(client, int.MaxValue / 2 + 1);
            client = await GetClientAsync();
            response = await GetLeaderboardAsync(client, "5-AroundMe");
            Assert.True(response.Entries.Any(e => e.Gamertag == theirGamertag));
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

        private async Task DeleteMyScores(HttpClient client)
        {
            await client.DeleteAsync(BaseUri);
        }

        private async Task<LeaderboardGetResponse> GetLeaderboardAsync(
            HttpClient client,
            string type = "Default",
            HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            HttpResponseMessage response = await client.GetAsync(BaseUri + "/" + type);
            Assert.Equal(expectedCode, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<LeaderboardGetResponse>(content);
        }

        private async Task PostScoreAsync(
            HttpClient client,
            int score,
            HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(BaseUri,
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