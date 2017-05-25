using Microsoft.Extensions.Logging;
using Nether.Integration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nether.Integration.Leaderboard
{
    /// <summary>
    /// Client used to allow the Leaderboard feature to integrate with Player Management
    /// in a pluggable manner
    /// </summary>
    public class DefaultLeaderboardPlayerManagementClient : BaseIntegrationClient, ILeaderboardPlayerManagementClient
    {
        private readonly ILogger _logger;

        public DefaultLeaderboardPlayerManagementClient(
            string identityBaseUri, // e.g. localhost:5000/identity
            string apiBaseUri, // e.g. localhost:5000/api
            string clientSecret,
            ILogger<DefaultLeaderboardPlayerManagementClient> logger
            )
            : base (
                  identityBaseUri,
                  apiBaseUri,
                  "nether_identity",
                  clientSecret,
                  logger)
        {
            _logger = logger;
        }

        public async Task<UserIdGamertagMap[]> GetGamertagsForUserIdsAsync(string[] userIds)
        {
            Func<HttpClient, Task<ApiResponse<UserIdGamertagMap[]>>> apiCall = async (client) =>
            {
                UserIdGamertagMap[] gamertags = null;
                var json = JsonConvert.SerializeObject(new { userIds });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var r = await client.PostAsync($"playeridentity/gamertags", content);
                if (r.IsSuccessStatusCode)
                {
                    var gamertagResponse = await ParseAsAsync<GamertagsResponseMessage>(r.Content);
                    gamertags = gamertagResponse.Gamertags;
                }
                return new ApiResponse<UserIdGamertagMap[]>
                {
                    StatusCode = r.StatusCode,
                    Success = r.IsSuccessStatusCode,
                    Result = gamertags
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

        private class GamertagsResponseMessage
        {
            public UserIdGamertagMap[] Gamertags { get; set; }
        }
    }
}
