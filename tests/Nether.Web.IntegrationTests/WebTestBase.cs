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

namespace Nether.Web.IntegrationTests
{
    /// <summary>
    /// Base class for integration test giving you an authenticated client ready to work with
    /// </summary>
    public class WebTestBase
    {
        public const string PlayerUser = "testuser";
        public const string AdminUser = "devadmin";

        protected const string BaseUrl = "http://localhost:5000/";
        private const string ClientId = "resourceowner-test";
        private const string ClientSecret = "devsecret";
        protected string _gamertag;

        private static readonly Dictionary<string, string> s_userToPassword =
            new Dictionary<string, string>
            {
                { "testuser", "testuser" },     // in "Player" role
                { "testuser1", "testuser1" },   // in "Player" role
                { "devadmin", "devadmin" }      // not in "Player" role
            };

        private static HttpClient CreateClient(string baseUrl)
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = new System.Net.CookieContainer()
            };

            return new HttpClient(handler) { BaseAddress = new Uri(baseUrl) };
        }

        protected async Task<HttpClient> GetAdminClientAsync()
        {
            return await GetClientAsync("devadmin", setPlayerGamertag: false);
        }

        protected async Task<HttpClient> GetClientAsync(string username = "testuser", bool setPlayerGamertag = true, string password = null)
        {
            HttpClient client = CreateClient(BaseUrl);
            if (password == null)
                password = s_userToPassword[username];

            //authenticate so it's ready for use
            DiscoveryResponse disco = await DiscoveryClient.GetAsync(BaseUrl);
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

            if (setPlayerGamertag)
            {
                _gamertag = username + "GamerTag";
                var player = new
                {
                    gamertag = _gamertag,
                    country = "UK",
                    customTag = nameof(WebTestBase)
                };
                HttpResponseMessage response = await client.PutAsJsonAsync("api/player", player);
                if (!response.IsSuccessStatusCode)
                {
                    throw new AuthenticationException("GetClient: could not update player info");
                }

                //get the token again as it will include the gamertag claim
                tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(username, password, "nether-all");
                client.SetBearerToken(tokenResponse.AccessToken);
            }

            return client;
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
