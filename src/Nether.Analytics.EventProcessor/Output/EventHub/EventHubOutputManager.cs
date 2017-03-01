// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Threading.Tasks;

namespace Nether.Analytics.EventProcessor.Output.EventHub
{
    public class EventHubOutputManager
    {
        private readonly EventHubClient _eventHubClient;

        public EventHubOutputManager(string eventHubConnectionString, string eventHubName)
        {
            _eventHubClient = EventHubClient.CreateFromConnectionString(eventHubConnectionString, eventHubName);
        }

        public async Task SendToEventHubAsync(GameEventData data, string line)
        {
            var now = DateTime.UtcNow;
            var delay = now - data.EnqueuedTime;

            //TODO: Discuss if this check is necessary at all

            // Only send events to intermediate EventHub if events are newer than 90 seconds
            // This prevents flooding of events if Event Processor has been turned off for a longer
            // period of time
            if (delay.TotalSeconds > 90)
                return;


            Console.WriteLine($"Sending to EventHub:");
            Console.WriteLine(line);

            //TODO: Implement Async Sending and caching of connection
            await _eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(line)));
        }
    }
}