// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
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

        public DefaultIdentityPlayerManagementClient(
            string baseUri, // e.g. localhost:5000
            ILoggerFactory loggerFactory
            )
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUri)
            };
            _logger = loggerFactory.CreateLogger<DefaultIdentityPlayerManagementClient>();
        }
        public async Task<string> GetGamertagForUserIdAsync(string userId)
        {
            // TODO security, proper url...;-)
            var response = await _httpClient.GetAsync($"/api/EVIL/HELPER/tagfromid/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var gamertag = await response.Content.ReadAsStringAsync();
                return gamertag;
            }
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            _logger.LogError("Failed to look up gamertag for user '{0}', statuscode {1}", userId, response.StatusCode);
            return null; // TODO should we throw here?
        }
    }
}
