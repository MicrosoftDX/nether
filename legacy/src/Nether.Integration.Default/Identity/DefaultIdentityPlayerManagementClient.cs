// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nether.Integration.Identity
{
    /// <summary>
    /// Client used to allow the Identity feature to integrate with Player Management
    /// in a pluggable manner
    /// </summary>
    public class DefaultIdentityPlayerManagementClient : BaseIntegrationClient, IIdentityPlayerManagementClient
    {
        private readonly ILogger _logger;

        public DefaultIdentityPlayerManagementClient(
            string identityBaseUri, // e.g. localhost:5000/identity
            string apiBaseUri, // e.g. localhost:5000/api
            string clientSecret,
            ILogger<DefaultIdentityPlayerManagementClient> logger
            )
            : base(
                  identityBaseUri,
                  apiBaseUri,
                  "nether_identity",
                  clientSecret,
                  logger)
        {
            _logger = logger;
        }
        public async Task<string> GetGamertagForUserIdAsync(string userId)
        {
            Func<HttpClient, Task<ApiResponse<string>>> apiCall = async (client) =>
            {
                string gamertag = null;
                var r = await client.GetAsync($"playeridentity/player/{userId}");
                if (r.IsSuccessStatusCode)
                {
                    var gamertagResponse = await ParseAsAsync<GetGamertagResponseMessage>(r.Content);
                    gamertag = gamertagResponse.Gamertag;
                }
                return new ApiResponse<string>
                {
                    Result = gamertag,
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
            return response.Result;
        }
        public async Task SetGamertagforUserIdAsync(string userId, string gamertag)
        {
            Func<HttpClient, Task<ApiResponse<object>>> apiCall = async (client) =>
            {
                var json = JsonConvert.SerializeObject(new { gamertag });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var r = await client.PostAsync($"playeridentity/player/{userId}", content);
                return new ApiResponse<object>
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
            Func<HttpClient, Task<ApiResponse<bool>>> apiCall = async (client) =>
            {
                var r = await client.GetAsync($"playeridentity/gamertag/{gamertag}");
                bool available = false;
                if (r.IsSuccessStatusCode)
                {
                    var gamertagResponse = await ParseAsAsync<GetGamertagAvailableResponseMessage>(r.Content);
                    available = gamertagResponse.Available;
                }
                return new ApiResponse<bool>
                {
                    StatusCode = r.StatusCode,
                    Success = r.IsSuccessStatusCode,
                    Result = available
                };
            };
            var response = await CallApiAsync(apiCall, "TestGamerTag");
            if (!response.Success)
            {
                _logger.LogError("Failed to test gamertag existence. Status code = '{0}'", response.StatusCode);
                throw new Exception($"Failed to test gamertag existence. Status code = {response.StatusCode}");
            }
            return response.Result;
        }

        private class GetGamertagResponseMessage
        {
            public string Gamertag { get; set; }
        }

        private class GetGamertagAvailableResponseMessage
        {
            public bool Available { get; set; }
        }
    }
}
