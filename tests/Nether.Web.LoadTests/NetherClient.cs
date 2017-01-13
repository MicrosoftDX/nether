// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace Nether.Web.LoadTests
{
    // This could form the basis of the .NET Client SDK for Nether
    public class NetherClient
    {
        private string _accessToken;
        private readonly string _baseUrl;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly HttpClient _httpClient;

        // TODO - figure out configuration to avoid these hard-coded strings
        public NetherClient(
            string baseUrl = "http://localhost:5000",
            string clientId = "resourceowner-test",
            string clientSecret = "devsecret")
        {
            _baseUrl = baseUrl;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _httpClient = CreateClient(baseUrl);
        }

        private HttpClient CreateClient(string baseUrl)
        {
            //configure client to use cookies                                  

            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = new System.Net.CookieContainer()
            };

            return new HttpClient(handler) { BaseAddress = new Uri(baseUrl) };
        }

        public async Task<OperationResult> LoginUserNamePasswordAsync(string username, string password)
        {
            // TODO - create a type so that the caller can determine success/failure (with message)
            var disco = await DiscoveryClient.GetAsync(_baseUrl);

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, _clientId, _clientSecret);
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(username, password, "nether-all");

            if (tokenResponse.IsError)
            {
                _accessToken = null;
                _httpClient.SetBearerToken(null);
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = tokenResponse.Error
                };
            }
            else
            {
                _accessToken = tokenResponse.AccessToken;
                _httpClient.SetBearerToken(_accessToken);

                AssignGamertag(tokenClient, username, password);

                return new OperationResult { IsSuccess = true };
            }
        }

        private void AssignGamertag(TokenClient client, string username, string password)
        {
            string gamertag = username + "GamerTag";
            var player = new
            {
                gamertag = gamertag,
                country = "UK",
                customtag = "LoadTest"
            };
            HttpResponseMessage response = _httpClient.PutAsJsonAsync("api/player", player).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new AuthenticationException("GetClient: could not update player info " + username);
            }

            //get the token again as it will include the gamertag claim
            var tokenResponse = client.RequestResourceOwnerPasswordAsync(username, password, "nether-all").Result;
            _httpClient.SetBearerToken(tokenResponse.AccessToken);
        }

        // TODO - create result model rather than returning JSON string!
        public async Task<OperationResult<string>> GetScoresAsync(string leaderboardType = null)
        {
            string uri = "/api/leaderboard";
            if (leaderboardType != null)
            {
                uri += "/" + leaderboardType;
            }

            var response = await _httpClient.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                return new OperationResult<string> { IsSuccess = true, Result = content };
            }
            else
            {
                return new OperationResult<string> { IsSuccess = false, Message = response.StatusCode.ToString() }; // TODO - read message body for error?
            }
        }

        public async Task<OperationResult> PostScoreAsync(int score)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "/api/leaderboard",
                new
                {
                    country = "missing",
                    customTag = "testclient",
                    score = score
                }
            );

            if (response.IsSuccessStatusCode)
            {
                return new OperationResult { IsSuccess = true };
            }
            else
            {
                return new OperationResult { IsSuccess = false, Message = response.StatusCode.ToString() }; // TODO - read message body for error?
            }
        }
    }

    public class OperationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
    public class OperationResult<T> : OperationResult
    {
        public T Result { get; set; }
    }
}
