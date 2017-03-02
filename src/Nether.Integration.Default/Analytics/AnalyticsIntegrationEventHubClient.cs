// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Nether.Analytics.GameEvents;
using Nether.Integration.Analytics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Microsoft.Azure.EventHubs;

namespace Nether.Integration.Default.Analytics
{
    public class AnalyticsIntegrationEventHubClient : IAnalyticsIntegrationClient
    {
        private readonly EventHubClient _eventHubClient;

        public AnalyticsIntegrationEventHubClient(string eventhubConnectionString)
        {
            _eventHubClient = EventHubClient.CreateFromConnectionString(eventhubConnectionString);
        }

        public async Task SendGameEventAsync(IGameEvent gameEvent)
        {
            var json = JsonConvert.SerializeObject(gameEvent);
            var eventData = new EventData(Encoding.UTF8.GetBytes(json));
            await _eventHubClient.SendAsync(eventData);
        }
    }
}

