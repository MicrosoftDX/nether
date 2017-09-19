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

namespace Nether.Integration
{
    /// <summary>
    /// Base class for integration clients that handles retrying connections
    /// </summary>
    public abstract class BaseIntegrationClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        private readonly string _clientId;
        private readonly string _clientSecret;

        // healthy flag and lock are used to avoid making multiple parallel failed calls when auth has failed and needs refreshing
        private AsyncLock _healthLock = new AsyncLock();
        private bool _healthy = false;
        private readonly string _identityBaseUri;

        protected BaseIntegrationClient(
            string identityBaseUri, // e.g. localhost:5000/identity
            string apiBaseUri, // e.g. localhost:5000/api
            string clientId,
            string clientSecret,
            ILogger logger
            )
        {
            _identityBaseUri = identityBaseUri.EnsureEndsWith("/");
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(apiBaseUri.EnsureEndsWith("/"))
            };

            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException($"{nameof(clientId)} must be specified", nameof(clientId));
            }
            _clientId = clientId;

            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentException($"{nameof(clientSecret)} must be specified", nameof(clientSecret));
            }
            _clientSecret = clientSecret;

            _logger = logger;
        }

        /// <summary>
        /// Helper to handle retrying auth (e.g. if token has expired)
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="apiCall"></param>
        /// <param name="apiName"></param>
        /// <param name="retryAuth"></param>
        /// <returns></returns>
        protected async Task<ApiResponse<T>> CallApiAsync<T>(Func<HttpClient, Task<ApiResponse<T>>> apiCall, string apiName)
        {
            ApiResponse<T> response = null;
            if (_healthy) // if healthy then proceed
            {
                response = await apiCall(_httpClient);
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
                    response = await apiCall(_httpClient);
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
                        response = await apiCall(_httpClient);
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

        protected class ApiResponse<T>
        {
            public HttpStatusCode StatusCode;
            public bool Success;
            public T Result { get; set; }
        }

        private async Task<TokenResponse> GetTokenAsync()
        {
            _logger.LogInformation("Attempting to get access token...");

            _logger.LogInformation("Querying token endpoint");
            var disco = await DiscoveryClient.GetAsync(_identityBaseUri);

            // request token
            _logger.LogInformation("Requesting token");
            var tokenClient = new TokenClient(disco.TokenEndpoint, _clientId, _clientSecret);
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

        protected async Task<T> ParseAsAsync<T>(HttpContent content)
        {
            // This would use System.Net.Http.Formatting which is in the Microsoft.AspNet.WebApi.Client package
            // but at the point of writing that doesn't support netstandard1.6

            var contentString = await content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<T>(contentString);
            return responseObject;
        }
    }
}
