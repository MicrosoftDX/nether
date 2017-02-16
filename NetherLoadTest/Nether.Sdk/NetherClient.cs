// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using IdentityModel.Client;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Nether.Sdk
{
    public class NetherClient
    {
        private readonly string _baseUrl;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly HttpClient _httpClient;

        public NetherClient(string baseUrl, string clientId, string clientSecret)
        {
            _baseUrl = baseUrl;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _httpClient = CreateClient(baseUrl);
        }

        private HttpClient CreateClient(string baseUrl)
        {
            return new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        private string _accessToken_Internal;
        public string AccessToken
        {
            get { return _accessToken_Internal; }
            set
            {
                _accessToken_Internal = value;
                _httpClient.SetBearerToken(_accessToken_Internal);
            }
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
                AccessToken = null;
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = tokenResponse.Error
                };
            }
            else
            {
                AccessToken = tokenResponse.AccessToken;

                return new OperationResult { IsSuccess = true };
            }
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
            var json = JsonConvert.SerializeObject(new
            {
                country = "missing",
                customTag = "testclient",
                score = score
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                "/api/leaderboard",
                content
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

