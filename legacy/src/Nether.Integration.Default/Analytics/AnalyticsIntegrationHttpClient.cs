// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Nether.Integration.Analytics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nether.Integration.Default.Analytics
{
    /// <summary>
    /// This class queries the /api/endpoint information to determine where to send analytics events
    /// This implementation has the broadest compatibility with the endpoint
    /// If using EventHub for the analytics events then use the AnalyticsInge
    /// </summary>
    public class AnalyticsIntegrationHttpClient : IAnalyticsIntegrationClient
    {
        private string _eventHubUrl;
        private string _authorization;
        private readonly string _analyticsBaseUrl;

        public AnalyticsIntegrationHttpClient(string analyticsBaseUrl)
        {
            // Make sure configured URL always ends with a "slash"
            if (!analyticsBaseUrl.EndsWith("/"))
                analyticsBaseUrl += "/";

            _analyticsBaseUrl = analyticsBaseUrl;
        }

        public async Task SendGameEventAsync(INetherMessage gameEvent)
        {
            // Retrieve information of where to send analytics data
            //TODO: Cache data so we don't ask everytime
            await GetAnalyticsEndpointInformation();

            // Setup client to call Event Hub
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(_authorization);

            // Create content to send
            var content = new StringContent(
                JsonConvert.SerializeObject(gameEvent),
                Encoding.UTF8, "application/json");

            //TODO: Implement retry logic and handling of exceptions
            await httpClient.PostAsync(_eventHubUrl, content);
        }

        #region Private helper methods

        private async Task GetAnalyticsEndpointInformation()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(_analyticsBaseUrl + "endpoint");

            //TODO: Consider re-using the response model object for the EndpointController instead of manually parsing the json response
            var jsonResponse = JObject.Parse(await response.Content.ReadAsStringAsync());
            _eventHubUrl = (string)jsonResponse["url"];
            _authorization = (string)jsonResponse["authorization"];

            //TODO: Implement logic to cache endpoint information and also request new authorization token if the old one times out
        }

        #endregion
    }
}

