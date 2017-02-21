// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using Microsoft.ServiceBus.Messaging;
using System;

namespace Nether.Analytics.EventProcessor.Output.EventHub
{
    public class EventHubOutputManager
    {
        private readonly string _eventHubConnectionString;
        private readonly string _eventHubName;

        public EventHubOutputManager(string eventHubConnectionString, string eventHubName)
        {
            _eventHubConnectionString = eventHubConnectionString;
            _eventHubName = eventHubName;
        }

        public void SendToEventHub(GameEventData data, string line)
        {
            var now = DateTime.UtcNow;
            var delay = now - data.EnqueuedTime;

            //TODO: Discuss if this check is necessary at all

            // Only send events to intermediate EventHub if events are newer than 90 seconds
            // This prevents flooding of events if Event Processor has been turned off for a longer
            // period of time
            if (delay.TotalSeconds > 90)
                return;

            var client = EventHubClient.CreateFromConnectionString(_eventHubConnectionString, _eventHubName);

            Console.WriteLine($"Sending to EventHub:");
            Console.WriteLine(line);

            //TODO: Implement Async Sending and caching of connection
            client.Send(new EventData(Encoding.UTF8.GetBytes(line)));
        }
    }
}