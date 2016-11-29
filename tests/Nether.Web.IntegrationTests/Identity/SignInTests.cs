using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace Nether.Web.IntegrationTests.Identity
{
    public class SignInTests
    {
        const string BaseUrl = "http://localhost:5000/";

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

            // TODO - inspect the token to check that the gamertag is NOT set

            // Set the Bearer token on subsequent requests
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenResult.AccessToken);

            // Should get a 404 NotFound for /player as there is no current player

            var playerResponse = await client.GetAsync("api/player");
            Assert.Equal(HttpStatusCode.NotFound, playerResponse.StatusCode);

            var player = new { gamertag = "testuser-notag", country="UK", customTag = "testing" };
            playerResponse = await client.PutAsJsonAsync("api/players/foo", player);
            playerResponse.EnsureSuccessStatusCode();

            playerResponse = await client.GetAsync("api/player");
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
