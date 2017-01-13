// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using IdentityModel.Client;
using Microsoft.Extensions.Logging;
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

        public DefaultIdentityPlayerManagementClient(
            string baseUri, // e.g. localhost:5000
            string clientSecret,
            ILoggerFactory loggerFactory
            )
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUri)
            };
            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentException("clientSecret must be specified", nameof(clientSecret));
            }
            _clientSecret = clientSecret;
            _logger = loggerFactory.CreateLogger<DefaultIdentityPlayerManagementClient>();
        }
        public async Task<string> GetGamertagForUserIdAsync(string userId)
        {
            return await GetGamertagForUserIdInternalAsync(userId, retryAuth: true);
        }

        private async Task<string> GetGamertagForUserIdInternalAsync(string userId, bool retryAuth)
        {
            // TODO security ;-)
            var response = await _httpClient.GetAsync($"/api/playertag/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var gamertagResponse = await ParseGamerTagResponseAsync(response.Content);
                return gamertagResponse.Gamertag;
            }
            if (response.StatusCode == HttpStatusCode.Forbidden
                || response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _logger.LogInformation("Attempt to get gamertag from userid returned status code {0}", response.StatusCode);
                if (retryAuth) // TODO - need to think about thread safety here...
                {
                    _logger.LogInformation("Retrying auth...");

                    var tokenResponse = await GetTokenAsync();
                    if (tokenResponse.IsError)
                    {
                        _logger.LogCritical("Failed to get token to call PlayerManagement!");
                        throw new Exception("Failed to get token to call PlayerManagement - unable to look up gamertags");
                    }
                    else
                    {
                        _logger.LogInformation("Success - retrying API call");
                        _httpClient.SetBearerToken(tokenResponse.AccessToken);
                        // retry the API call!
                        return await GetGamertagForUserIdInternalAsync(userId, retryAuth: false); // set retryAuth to false to avoid infinite loop retrying
                    }
                }
                else
                {
                    _logger.LogCritical("Auth error calling PlayerManagement. RetryAuth set to false");
                    throw new Exception("Auth error calling PlayerManagement. RetryAuth set to false");
                }
            }
            _logger.LogCritical("Failed to look up gamertag with statuscode {0} for '{1}'", response.StatusCode, response.RequestMessage.RequestUri);
            throw new Exception($"Failed to look up gamertag with statuscode {response.StatusCode} for '{response.RequestMessage.RequestUri}'");
        }

      

        private async Task<TokenResponse> GetTokenAsync()
        {
            _logger.LogInformation("Attempting to get access token...");

            _logger.LogInformation("Querying token endpoint");
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");

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

        private async Task<GamerTagResponse> ParseGamerTagResponseAsync(HttpContent content)
        {
            // This would use System.Net.Http.Formatting which is in the Microsoft.AspNet.WebApi.Client package
            // but at the point of writing that doesn't support netstandard1.6

            var contentString = await content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<GamerTagResponse>(contentString);
            return responseObject;
        }
        private class GamerTagResponse
        {
            public string Gamertag { get; set; }
        }
    }
}
