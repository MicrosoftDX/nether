// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using Microsoft.ServiceBus.Messaging;

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

        public void SendToEventHub(string gameEventType, string data)
        {
            // All events are sent to the same Event Hub, so no need to look at gameEventType at this time

            var client = EventHubClient.CreateFromConnectionString(_eventHubConnectionString, _eventHubName);

            //TODO: Implement Async Sending and caching of connection
            client.Send(new EventData(Encoding.UTF8.GetBytes(data)));
        }
    }
}