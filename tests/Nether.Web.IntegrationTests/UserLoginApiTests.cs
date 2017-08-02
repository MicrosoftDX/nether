// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Nether.Web.IntegrationTests
{
    public class UserLoginApiTests : WebTestBase, IClassFixture<IntegrationTestUsersFixture>
    {
        [Fact]
        public async Task A_user_can_sign_up_as_a_guest_post_a_score_add_another_login_and_then_retrieve_the_score()
        {
            var guestId = Guid.NewGuid().ToString("N");
            var client = await SignInAsGuestAsync(guestId);

            // PUT /api/players [set gamertag]
            var player = new { gamertag = $"gamertag-{guestId}", country = "UK", customTag = "IntegrationTestGuestUser" };
            var playerResponse = await client.PutAsJsonAsync("api/player", player);
            await playerResponse.AssertSuccessStatusCodeAsync();

            // we need to do this so that we can get the new token *with* the gamertag
            client = await SignInAsGuestAsync(guestId);

            // POST /api/scores
            var scoreValue = new Random().Next(100, 999);
            var score = new { score = scoreValue };
            var scoreResponse = await client.PostAsJsonAsync("api/scores", score);
            await scoreResponse.AssertStatusCodeAsync(HttpStatusCode.OK);

            // Add username + password
            var username = $"user-{guestId}";
            var password = "test-password";
            var usernameResponse = await client.PutAsJsonAsync("api/user/logins/password", new { username, password });
            await usernameResponse.AssertSuccessStatusCodeAsync();

            // sign in using username+password
            client = await SignInAsync(username, password);

            // GET /api/leadeboards/Default
            var leaderboardResponse = await client.GetAsync("api/leaderboards/Default");

            dynamic content = await leaderboardResponse.Content.ReadAsAsync<dynamic>();
            Assert.NotNull(content.currentPlayer);

            Assert.Equal(scoreValue, (int)content.currentPlayer.score);
        }
    }
}