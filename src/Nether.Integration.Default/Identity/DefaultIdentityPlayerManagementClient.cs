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
            return await GetGamertagForUserIdInternalAsync(userId, retryAuth: true);
        }

        private async Task<string> GetGamertagForUserIdInternalAsync(string userId, bool retryAuth)
        {
            // TODO security ;-)
            GamerTagResponse response = null;
            if (_healthy) // if healthy then proceed
            {
                response = await CallGamertagApiAsync(userId);
            }

            // if not healthy or the last call failed with auth issues..
            // use an async-safe lock to coordinate checks and updates of _healthy flag
            using (var releaser = await _healthLock.LockAsync())
            {
                // double-check the healthy flag under the lock
                if (_healthy) // if healthy then proceed
                {
                    response = await CallGamertagApiAsync(userId);
                    if (response.SuccessStatusCode)
                    {
                        return response.Gamertag;
                    }
                    _logger.LogInformation("Attempt to get gamertag from userid returned status code {0}", response.StatusCode);
                }

                if (!_healthy
                || response?.StatusCode == HttpStatusCode.Forbidden
                || response?.StatusCode == HttpStatusCode.Unauthorized)
                {
                    if (retryAuth) // TODO - need to think about thread safety here...
                    {
                        _logger.LogInformation("Retrying auth...");

                        var tokenResponse = await GetTokenAsync();
                        if (tokenResponse.IsError)
                        {
                            _healthy = false;
                            _logger.LogCritical("Failed to get token to call PlayerManagement!");
                            throw new Exception("Failed to get token to call PlayerManagement - unable to look up gamertags");
                        }
                        else
                        {
                            _logger.LogInformation("Success - retrying API call");
                            _httpClient.SetBearerToken(tokenResponse.AccessToken);
                            // retry the API call!
                            response = await CallGamertagApiAsync(userId);
                            if (response.SuccessStatusCode)
                            {
                                _logger.LogInformation("Successfully called the API");
                                _healthy = true;
                                return response.Gamertag;
                            }
                            else
                            {
                                _logger.LogCritical("Failed to call PlayerManagement after refreshing token!");
                            }
                        }
                    }
                    else
                    {
                        _healthy = false;
                        _logger.LogCritical("Auth error calling PlayerManagement. RetryAuth set to false");
                        throw new Exception("Auth error calling PlayerManagement. RetryAuth set to false");
                    }
                }
                _healthy = false;
                _logger.LogCritical("Failed to look up gamertag with statuscode {0}", response.StatusCode);
                throw new Exception($"Failed to look up gamertag with statuscode {response.StatusCode}");
            }
        }

        private class GamerTagResponse
        {
            public string Gamertag;
            public HttpStatusCode StatusCode;
            public bool SuccessStatusCode;
        }
        private async Task<GamerTagResponse> CallGamertagApiAsync(string userId)
        {
            string gamertag = null;
            var response = await _httpClient.GetAsync($"playertag/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var gamertagResponse = await ParseGamerTagResponseAsync(response.Content);
                gamertag = gamertagResponse.Gamertag;
            }
            return new GamerTagResponse
            {
                Gamertag = gamertag,
                StatusCode = response.StatusCode,
                SuccessStatusCode = response.IsSuccessStatusCode
            };
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

        private async Task<GamerTagResponseMessage> ParseGamerTagResponseAsync(HttpContent content)
        {
            // This would use System.Net.Http.Formatting which is in the Microsoft.AspNet.WebApi.Client package
            // but at the point of writing that doesn't support netstandard1.6

            var contentString = await content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<GamerTagResponseMessage>(contentString);
            return responseObject;
        }
        private class GamerTagResponseMessage
        {
            public string Gamertag { get; set; }
        }
    }
}
