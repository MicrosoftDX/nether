// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.EventHubs;
using Nether.Analytics.MessageFormats;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

//TODO: A more advanced version of this should probably be available in Nether.Analytics and/or Nether.Analytics.EventHubs

namespace AnalyticsTestClient
{
    public class AnalyticsClient : IAnalyticsClient
    {
        private EventHubClient _client;

        public AnalyticsClient()
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(Config.Root[Config.NAH_EHLISTENER_CONNECTIONSTRING])
            {
                EntityPath = Config.Root[Config.NAH_EHLISTENER_EVENTHUBPATH]
            };

            _client = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
        }

        public async Task SendMessageAsync(INetherMessage msg, DateTime? dbgEnqueuedTimeUtc = null)
        {
            Console.Write("|");

            // Serialize object to JSON
            var json = JsonConvert.SerializeObject(
                                msg,
                                Formatting.Indented,
                                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            if (dbgEnqueuedTimeUtc.HasValue)
            {
                var o = JObject.Parse(json);
                o.Add("dbgEnqueuedTimeUtc", dbgEnqueuedTimeUtc);
                json = o.ToString();
            }

            await _client.SendAsync(new EventData(Encoding.UTF8.GetBytes(json)));
        }
    }

    public class BatchAnalyticsClient : IAnalyticsClient
    {
        private const int MaxMessagesInBatch = 1000;
        private const int MaxTotalBatchSizeInBytes = 200 * 1024; // Theoretical limit is 256KB
        private const int MaxMillisecondsBetweenBatches = 1000;

        private EventHubClient _client;
        private ConcurrentQueue<EventData> _queue = new ConcurrentQueue<EventData>();
        private Stopwatch _timeSinceLastSent = new Stopwatch();
        private int _batchSize = 0;

        public BatchAnalyticsClient()
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(Config.Root[Config.NAH_EHLISTENER_CONNECTIONSTRING])
            {
                EntityPath = Config.Root[Config.NAH_EHLISTENER_EVENTHUBPATH]
            };

            _client = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
            _timeSinceLastSent.Start();
        }

        public async Task SendMessageAsync(INetherMessage msg, DateTime? dbgEnqueuedTimeUtc = default(DateTime?))
        {
            // Serialize object to JSON
            var json = JsonConvert.SerializeObject(
                                msg,
                                Formatting.Indented,
                                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            if (dbgEnqueuedTimeUtc.HasValue)
            {
                var o = JObject.Parse(json);
                o.Add("dbgEnqueuedTimeUtc", dbgEnqueuedTimeUtc);
                json = o.ToString();
            }

            await SendMessageAsync(json);
        }

        public async Task SendMessageAsync(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            _batchSize += bytes.Length;

            var data = new EventData(bytes);
            _queue.Enqueue(data);
            Console.Write("q");

            if (_queue.Count >= MaxMessagesInBatch || _timeSinceLastSent.ElapsedMilliseconds > MaxMillisecondsBetweenBatches || _batchSize > MaxTotalBatchSizeInBytes)
                await FlushMessagesInQueueAsync();
        }

        public async Task FlushMessagesInQueueAsync()
        {
            if (_queue.Count > 0)
            {
                var list = new List<EventData>();

                while (_queue.TryDequeue(out var data))
                {
                    list.Add(data);
                }

                await _client.SendAsync(list);
                Console.Write("F");
                _timeSinceLastSent.Restart();
                _batchSize = 0;
            }
        }
    }
}