// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using IdentityModel;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace Nether.Web.IntegrationTests.Identity
{
    public class SignInTests : WebTestBase, IClassFixture<IntegrationTestUsersFixture>
    {
        [Fact]
        public async Task As_a_new_user_I_can_authenticate_and_create_a_gamertag()
        {
            const string username = "testuser-notag";
            const string password = "password123";

            var client = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };

            // Sign in as user without gamertag/profile
            var accessTokenResult = await GetAccessToken(client, username, password);
            if (accessTokenResult.Error != null)
            {
                throw new Exception("error in auth:" + accessTokenResult.Error);
            }
            Assert.NotNull(accessTokenResult.AccessToken);

            // inspect the token to check that the gamertag is NOT set
            var token = new JwtSecurityToken(accessTokenResult.AccessToken);
            var claim = token.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.NickName);
            Assert.Null(claim);

            // Set the Bearer token on subsequent requests
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenResult.AccessToken);

            // GET /api/player should get a 404 NotFound for /player as there is no current player
            var playerResponse = await client.GetAsync("api/player");
            Assert.Equal(HttpStatusCode.NotFound, playerResponse.StatusCode);

            // PUT /api/players
            var player = new { gamertag = "testuser-notag", country = "UK", customTag = "IntegrationTestUser" };
            playerResponse = await client.PutAsJsonAsync("api/player", player);
            playerResponse.EnsureSuccessStatusCode();

            // Reauthenticate
            accessTokenResult = await GetAccessToken(client, username, password);
            if (accessTokenResult.Error != null)
            {
                throw new Exception("error in auth:" + accessTokenResult.Error);
            }
            Assert.NotNull(accessTokenResult.AccessToken);

            // inspect the token to check that the gamertag IS set now
            token = new JwtSecurityToken(accessTokenResult.AccessToken);
            claim = token.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.NickName);
            Assert.NotNull(claim);
            Assert.Equal("testuser-notag", claim.Value);

            // Set the Bearer token on subsequent requests
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenResult.AccessToken);

            // GET /api/player
            playerResponse = await client.GetAsync("api/player");
            playerResponse.EnsureSuccessStatusCode();

            // GET /api/player
            var playerGroupsResponse = await client.GetAsync("api/player/groups");

            // DELETE /api/player
            playerResponse = await client.DeleteAsync("api/player");
            playerResponse.EnsureSuccessStatusCode();
        }




        private async Task<AccessTokenResult> GetAccessToken(HttpClient client, string username, string password)
        {
            const string client_id = "devclient";
            const string client_secret = "devsecret";
            const string scope = "openid profile nether-all";


            var requestBody = new FormUrlEncodedContent(
                  new Dictionary<string, string>
                  {
                        { "grant_type", "password" },
                        { "client_id",  client_id },
                        { "client_secret", client_secret },
                        { "username", username },
                        { "password", password },
                        { "scope", scope }
                  }
              );
            var response = await client.PostAsync("/connect/token", requestBody);
            dynamic responseBody = await response.Content.ReadAsAsync<dynamic>();


            if (responseBody.error != null)
            {
                return new AccessTokenResult { Error = responseBody.Error };
            }
            return new AccessTokenResult
            {
                AccessToken = (string)responseBody.access_token,
                ExpiresIn = (int)responseBody.expires_in
            };
        }
    }

    public class AccessTokenResult
    {
        public string Error { get; set; }
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
    }
}
