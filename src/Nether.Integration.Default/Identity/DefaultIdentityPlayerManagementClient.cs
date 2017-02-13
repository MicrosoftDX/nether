// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Nether.Common;
using Nether.Common.Async;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nether.Integration.Identity
{
    /// <summary>
    /// Client used to allow the Identity feature to integrate with Player Management
    /// in a pluggable manner
    /// </summary>
    public class DefaultIdentityPlayerManagementClient : IIdentityPlayerManagementClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DefaultIdentityPlayerManagementClient> _logger;

        private const string ClientId = "nether-identity";
        private readonly string _clientSecret;

        // healthy flag and lock are used to avoid making multiple parallel failed calls when auth has failed and needs refreshing
        private AsyncLock _healthLock = new AsyncLock();
        private bool _healthy = false;
        private readonly string _identityBaseUri;

        public DefaultIdentityPlayerManagementClient(
            string identityBaseUri, // e.g. localhost:5000/identity
            string apiBaseUri, // e.g. localhost:5000/api
            string clientSecret,
            ILogger<DefaultIdentityPlayerManagementClient> logger
            )
        {
            _identityBaseUri = identityBaseUri.EnsureEndsWith("/");
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(apiBaseUri.EnsureEndsWith("/"))
            };
            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentException("clientSecret must be specified", nameof(clientSecret));
            }
            _clientSecret = clientSecret;
            _logger = logger;
        }
        public async Task<string> GetGamertagForUserIdAsync(string userId)
        {
            Func<Task<GamertagResponse>> apiCall = async () =>
            {
                string gamertag = null;
                var r = await _httpClient.GetAsync($"playeridentity/player/{userId}");
                if (r.IsSuccessStatusCode)
                {
                    var gamertagResponse = await ParseGamerTagResponseAsync(r.Content);
                    gamertag = gamertagResponse.Gamertag;
                }
                return new GamertagResponse
                {
                    Gamertag = gamertag,
                    StatusCode = r.StatusCode,
                    Success = r.IsSuccessStatusCode
                };
            };
            var response = await CallApiAsync(apiCall, "GetGamertagFromUserId");
            if (!response.Success)
            {
                _logger.LogError("Failed to get gamertag for user. Status code = '{0}'", response.StatusCode);
                throw new Exception($"Failed to get gamertag for user. Status code = {response.StatusCode}");
            }
            return response.Gamertag;
        }
        public async Task SetGamertagforUserIdAsync(string userId, string gamertag)
        {
            Func<Task<ApiResponse>> apiCall = async () =>
            {
                var json = JsonConvert.SerializeObject(new { gamertag });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var r = await _httpClient.PostAsync($"playeridentity/player/{userId}", content);
                return new ApiResponse
                {
                    StatusCode = r.StatusCode,
                    Success = r.IsSuccessStatusCode
                };
            };
            var response = await CallApiAsync(apiCall, "SetGamertagForUserId");
            if (!response.Success)
            {
                _logger.LogError("Failed to set gamertag for user. Status code = '{0}'", response.StatusCode);
                throw new Exception($"Failed to set gamertag for user. Status code = {response.StatusCode}");
            }
        }

        public async Task<bool> GamertagIsAvailableAsync(string gamertag)
        {
            Func<Task<ApiResponse>> apiCall = async () =>
            {
                var r = await _httpClient.GetAsync($"playeridentity/gamertag/{gamertag}");
                return new ApiResponse
                {
                    StatusCode = r.StatusCode,
                    Success = r.IsSuccessStatusCode || r.StatusCode == HttpStatusCode.NotFound
                };
            };
            var response = await CallApiAsync(apiCall, "TestGamerTag");
            if (!response.Success)
            {
                _logger.LogError("Failed to test gamertag existence. Status code = '{0}'", response.StatusCode);
                throw new Exception($"Failed to test gamertag existence. Status code = {response.StatusCode}");
            }
            return response.StatusCode == HttpStatusCode.NotFound; // tag not found => tag available
        }

        /// <summary>
        /// Helper to handle retrying auth (e.g. if token has expired)
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="apiCall"></param>
        /// <param name="apiName"></param>
        /// <param name="retryAuth"></param>
        /// <returns></returns>
        private async Task<TResponse> CallApiAsync<TResponse>(Func<Task<TResponse>> apiCall, string apiName)
            where TResponse : ApiResponse
        {
            TResponse response = null;
            if (_healthy) // if healthy then proceed
            {
                response = await apiCall();
                if (response.StatusCode != HttpStatusCode.Forbidden
                    && response?.StatusCode != HttpStatusCode.Unauthorized)
                {
                    return response;
                }
            }

            // if not healthy or the last call failed with auth issues..
            // use an async-safe lock to coordinate checks and updates of _healthy flag
            using (var releaser = await _healthLock.LockAsync())
            {
                // double-check the healthy flag under the lock
                if (_healthy) // if healthy then proceed
                {
                    response = await apiCall();
                    if (response.StatusCode != HttpStatusCode.Forbidden
                        && response?.StatusCode != HttpStatusCode.Unauthorized)
                    {
                        return response;
                    }
                    _logger.LogInformation("Attempt to call PlayerManagementIdentity API '{0}' returned status code {1}", apiName, response.StatusCode);
                }

                if (!_healthy
                || response?.StatusCode == HttpStatusCode.Forbidden
                || response?.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _logger.LogInformation("Retrying auth...");

                    var tokenResponse = await GetTokenAsync();
                    if (tokenResponse.IsError)
                    {
                        _healthy = false;
                        _logger.LogCritical("Failed to get token to call PlayerManagementIdentity API '{0}'!", apiName);
                        throw new Exception($"Failed to get token to call PlayerManagementIdentity API '{apiName}'");
                    }
                    else
                    {
                        _logger.LogInformation("Success - retrying API call");
                        _httpClient.SetBearerToken(tokenResponse.AccessToken);
                        // retry the API call!
                        response = await apiCall();
                        if (response.StatusCode != HttpStatusCode.Forbidden
                            && response?.StatusCode != HttpStatusCode.Unauthorized)
                        {
                            _logger.LogInformation("Successfully called the API");
                            _healthy = true;
                            return response;
                        }
                        else
                        {
                            _logger.LogCritical("Failed to call PlayerManagementIdentity API '{0}' after refreshing token!", apiName);
                        }
                    }
                }
                _healthy = false; // TODO - we should consider some kind of back-off timeout here
                _logger.LogCritical("Failed to call PlayerManagementIdentity API '{0}' with statuscode {1}", apiName, response.StatusCode);
                throw new Exception($"Failed to call PlayerManagementIdentity API {apiName} with statuscode {response.StatusCode}");
            }
        }

        private class ApiResponse
        {
            public HttpStatusCode StatusCode;
            public bool Success;
        }
        private class GamertagResponse : ApiResponse
        {
            public string Gamertag;
        }

        private async Task<TokenResponse> GetTokenAsync()
        {
            _logger.LogInformation("Attempting to get access token...");

            _logger.LogInformation("Querying token endpoint");
            var disco = await DiscoveryClient.GetAsync(_identityBaseUri);

            // request token
            _logger.LogInformation("Requesting token");
            var tokenClient = new TokenClient(disco.TokenEndpoint, ClientId, _clientSecret);
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("nether-all");

            if (tokenResponse.IsError)
            {
                _logger.LogCritical("Failed to get token: '{0}'; '{1}'", tokenResponse.HttpStatusCode, tokenResponse.HttpErrorReason);
            }
            else
            {
                _logger.LogInformation("Got token");
            }
            return tokenResponse;
        }

        private async Task<GetGamerTagResponseMessage> ParseGamerTagResponseAsync(HttpContent content)
        {
            // This would use System.Net.Http.Formatting which is in the Microsoft.AspNet.WebApi.Client package
            // but at the point of writing that doesn't support netstandard1.6

            var contentString = await content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<GetGamerTagResponseMessage>(contentString);
            return responseObject;
        }

        private class GetGamerTagResponseMessage
        {
            public string Gamertag { get; set; }
        }
    }
}
