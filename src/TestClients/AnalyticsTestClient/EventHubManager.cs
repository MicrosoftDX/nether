// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;

namespace AnalyticsTestClient.Utils
{
    public static class EventHubManager
    {
        public static async Task SendMessageToEventHub(string msg)
        {
            Console.WriteLine($"Connecting to EventHub [{ConfigCache.EventHubName}]");
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(ConfigCache.EventHubConnectionString)
            {
                EntityPath = ConfigCache.EventHubName
            };

            var client = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            Console.WriteLine($"Sending message...");
            Console.WriteLine(msg);
            await client.SendAsync(new EventData(Encoding.UTF8.GetBytes(msg)));
            Console.WriteLine("Message sent!");
            await client.CloseAsync();
        }
    }
}