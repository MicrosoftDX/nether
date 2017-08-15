// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Authentication;
using IdentityModel.Client;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http.Headers;

namespace Nether.Web.IntegrationTests
{
    /// <summary>
    /// Base class for integration test giving you an authenticated client ready to work with
    /// </summary>
    public class WebTestBase
    {
        public const string PlayerUser = "testuser";
        public const string AdminUser = "devadmin";

        public static string BaseUrl => (Environment.GetEnvironmentVariable("NETHER_INTEGRATION_TEST_BASE")
            ?? "http://localhost:5000/").EnsureEndsWith("/");

        private const string ClientId = "resourceownertest";
        private const string ClientSecret = "devsecret";

        protected class UserInfo
        {
            public string UserName { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }
            public string Gamertag { get; internal set; }
        }
        protected static readonly UserInfo[] s_users = new[]
        {
            new UserInfo { UserName = "testuser", Password = "testuser", Role = "Player", Gamertag = "testuser" },
            new UserInfo { UserName = "testuser1", Password = "password123", Role = "Player", Gamertag = "testuser1" },
            new UserInfo { UserName = "testuser2", Password = "password123", Role = "Player", Gamertag = "testuser2" },
            new UserInfo { UserName = "testuser3", Password = "password123", Role = "Player", Gamertag = "testuser3" },
            new UserInfo { UserName = "testuser4", Password = "password123", Role = "Player", Gamertag = "testuser4" },
            new UserInfo { UserName = "testuser5", Password = "password123", Role = "Player", Gamertag = "testuser5" },
            new UserInfo { UserName = "testuser6", Password = "password123", Role = "Player", Gamertag = "testuser6" },
            new UserInfo { UserName = "testuser7", Password = "password123", Role = "Player", Gamertag = "testuser7" },
            new UserInfo { UserName = "testuser8", Password = "password123", Role = "Player", Gamertag = "testuser8" },
            new UserInfo { UserName = "testuser9", Password = "password123", Role = "Player", Gamertag = "testuser9" },
            new UserInfo { UserName = "testuser10", Password = "password123", Role = "Player", Gamertag = "testuser10" },
            new UserInfo { UserName = "testuser11", Password = "password123", Role = "Player", Gamertag = "testuser11" },
            new UserInfo { UserName = "testuser12", Password = "password123", Role = "Player", Gamertag = "testuser12" },
            new UserInfo { UserName = "testuser-state", Password = "password123", Role = "Player", Gamertag = "testuser-state" },
            new UserInfo { UserName = "testuser-notag", Password = "password123", Role = "Player" },
            new UserInfo { UserName = "testuser-notag2", Password = "password123", Role = "Player" },
            new UserInfo { UserName = "devadmin", Password = "devadmin", Role = "Admin" },
        };

        private static HttpClient CreateClient(string baseUrl)
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = new CookieContainer()
            };

            return new HttpClient(handler) { BaseAddress = new Uri(baseUrl) };
        }

        protected async Task<HttpClient> SignInAsAdminAsync()
        {
            return await SignInAsync("devadmin");
        }

        protected async Task<HttpClient> SignInAsync(string username = "testuser", string password = null)
        {
            HttpClient client = CreateClient(BaseUrl);
            if (password == null)
                password = GetPassword(username);

            //authenticate so it's ready for use
            DiscoveryResponse disco = await DiscoveryClient.GetAsync($"{BaseUrl}identity");
            if (disco.TokenEndpoint == null)
            {
                throw new AuthenticationException("GetClient: could not discover endpoint, server is offline?");
            }

            var tokenClient = new TokenClient(disco.TokenEndpoint, ClientId, ClientSecret);


            TokenResponse tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(username, password, "nether-all");
            if (tokenResponse.IsError)
            {
                throw new AuthenticationException($"GetClient: failed to authenticate: '{tokenResponse.ErrorDescription}'");
            }
            client.SetBearerToken(tokenResponse.AccessToken);

            return client;
        }



        protected async Task<HttpClient> SignInAsGuestAsync(string guestId)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };

            // Sign in as a guest
            var accessTokenResult = await GetGuestAccesstoken(client, guestId);

            if (accessTokenResult.Error != null)
            {
                throw new Exception("Error in auth:" + accessTokenResult.Error);
            }

            Assert.NotNull(accessTokenResult.AccessToken);

            // Set the Bearer token on subsequent requests
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenResult.AccessToken);
            return client;
        }

        protected async Task<AccessTokenResult> GetAccessToken(HttpClient client, string username, string password)
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

            return await MakeTokenRequestAsync(client, requestBody);
        }

        protected async Task<AccessTokenResult> GetGuestAccesstoken(HttpClient client, string guestIdentifier)
        {
            const string client_id = "devclient";
            const string client_secret = "devsecret";
            const string scope = "openid profile nether-all";


            var requestBody = new FormUrlEncodedContent(
                  new Dictionary<string, string>
                  {
                        { "grant_type", "guest-access" },
                        { "client_id",  client_id },
                        { "client_secret", client_secret },
                        { "guest_identifier", guestIdentifier },
                        { "scope", scope }
                  }
              );

            return await MakeTokenRequestAsync(client, requestBody);
        }

        private static async Task<AccessTokenResult> MakeTokenRequestAsync(HttpClient client, FormUrlEncodedContent requestBody)
        {
            var response = await client.PostAsync("/identity/connect/token", requestBody);
            dynamic responseBody = await response.Content.ReadAsAsync<dynamic>();


            if (responseBody.error != null)
            {
                return new AccessTokenResult { Error = responseBody.error };
            }
            return new AccessTokenResult
            {
                AccessToken = (string)responseBody.access_token,
                ExpiresIn = (int)responseBody.expires_in
            };
        }



        public class AccessTokenResult
        {
            public string Error { get; set; }
            public string AccessToken { get; set; }
            public int ExpiresIn { get; set; }
        }
        private string GetPassword(string username)
        {
            return s_users.FirstOrDefault(u => u.UserName == username)?.Password;
        }

        protected async Task<T> HttpGet<T>(HttpClient client, string url, HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            HttpResponseMessage response = await client.GetAsync(url);

            Assert.Equal(expectedCode, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}
