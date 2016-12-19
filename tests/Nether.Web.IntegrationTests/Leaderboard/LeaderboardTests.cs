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
        private HttpClient _client;

        public LeaderboardTests()
        {
            _client = GetClient();
        }

        [Fact]
        public async Task Get_leaderboard_call_succeeds()
        {
            HttpResponseMessage response = await _client.GetAsync(BaseUri + "/Default");
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Post_leaderboard_call_succeeds()
        {
            await PostScore(1);
        }

        [Fact]
        public async Task Posting_new_score_updates_default_leaderboard()
        {
            LeaderboardGetResponse leaderboardBefore = await GetLeaderboard();

            List<LeaderboardGetResponse.LeaderboardEntry> entries =
                leaderboardBefore.Entries.Where(e => e.Gamertag == _gamertag).ToList();

            //check that there is only one or less (if score wasn't posted yet) entry per user
            Assert.True(entries.Count <= 1);
            int oldScore = entries.Count == 0 ? 0 : entries.First().Score;

            //update the score posting a different (higher) result
            int newScore = oldScore + 1;
            await PostScore(newScore);

            //check that leaderboard has the updated score
            LeaderboardGetResponse leaderboardAfter = await GetLeaderboard();
            int newFreshScore = leaderboardAfter.Entries.Where(e => e.Gamertag == _gamertag).Select(e => e.Score).First();
            Assert.Equal(newFreshScore, newScore);
        }

        [Fact]
        public async Task Posting_similar_score_gets_around_me()
        {
            //note: radius is 2 at the moment, meaning you get 2 players above and 2 below (4 elements in response in general)

            //check there are at least 5 users
            LeaderboardGetResponse response = await GetLeaderboard();
            if (response.Entries.Length < 5)
            {
                throw new NotImplementedException();    //todo: post scores to get at least 5
            }

            //todo: delete score entries before testing, this requires a separate http method

            //put me somewhere in the middle and push the other user in the bottom so he is not around me
            await DeleteMyScores();
            await PostScore(int.MaxValue / 2);
            string myGamertag = _gamertag;
            _client = GetClient("testuser1");
            string hisGamertag = _gamertag;
            await DeleteMyScores();
            await PostScore(1);

            //check he is not around me
            _client = GetClient();
            response = await GetLeaderboard("Around Me");
            Assert.False(response.Entries.Any(e => e.Gamertag == hisGamertag));

            //make his score similar to mine and check he is around me
            _client = GetClient("testuser1");
            await PostScore(int.MaxValue / 2 + 1);
            _client = GetClient();
            response = await GetLeaderboard("Around Me");
            Assert.True(response.Entries.Any(e => e.Gamertag == hisGamertag));
        }

        [Fact]
        public async Task Limiting_top_scores_returns_limited_numer_of_rows()
        {
            LeaderboardGetResponse response = await GetLeaderboard("Top", HttpStatusCode.OK);

            Assert.True(response.Entries.Length <= 5);
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
            _client = GetAdminClient();    //login as devadmin who is not in "Player" role

            await GetLeaderboard("Default", HttpStatusCode.Forbidden);
        }

        #region [ REST Wrappers ]

        private async Task DeleteMyScores()
        {
            await _client.DeleteAsync(BaseUri);
        }

        private async Task<LeaderboardGetResponse> GetLeaderboard(string type = "Default",
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

                public override string ToString()
                {
                    return $"{Gamertag}\t {Score}";
                }
            }
        }

        #endregion
    }
}