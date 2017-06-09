// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using System.Text;

namespace Nether.Analytics
{
    public class EventHubOutputManager : IOutputManager
    {
        private string _ehConnectionString;
        private IMessageFormatter _serializer;
        private EventHubClient _client;

        /// <summary>
        /// Creates a new instance, with a JSON output formatter as the default serializer.
        /// </summary>
        /// <param name="outputEventHubConnectionString">The connection string for the event hub output.</param>
        public EventHubOutputManager(string outputEventHubConnectionString)
            : this(outputEventHubConnectionString, new JsonMessageFormatter())
        {
        }

        public EventHubOutputManager(string outputEventHubConnectionString, IMessageFormatter serializer)
        {
            _serializer = serializer;
            _ehConnectionString = outputEventHubConnectionString;
        }

        private void EnsureEventHubClient()
        {
            if (_client == null)
            {
                if (string.IsNullOrEmpty(_ehConnectionString))
                {
                    throw new ArgumentException("Missing Event Hub connection string.");
                }

                _client = EventHubClient.CreateFromConnectionString(_ehConnectionString);
            }
        }
        private EventHubClient Client
        {
            get
            {
                EnsureEventHubClient();
                return _client;
            }
        }

        public Task OutputMessageAsync(string partitionId, string pipelineName, int index, Message msg)
        {
            string payload = _serializer.Format(msg);

            if (_serializer.IncludeHeaders)
            {
                payload = $"{_serializer.Header}{Environment.NewLine}{payload}";
            }

            var eventData = new EventData(Encoding.UTF8.GetBytes(payload));

            return Client.SendAsync(eventData);
        }

        public Task FlushAsync(string partitionId)
        {
            // this client does not "support" flushing, per-se, but we don't
            // want to throw an exception, so we're just "ignoring" this
            return Task.CompletedTask;
        }
    }
}