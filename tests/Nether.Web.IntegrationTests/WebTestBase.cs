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

        protected async Task<HttpClient> GetAdminClientAsync()
        {
            return await GetClientAsync("devadmin");
        }

        protected async Task<HttpClient> GetClientAsync(string username = "testuser", string password = null)
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
