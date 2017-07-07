// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.EventHubs;
using Nether.Ingest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nether.EventHubs
{
    public class EventHubAnalyticsClientOptions
    {
        public int MaxBatchSize { get; set; } = 500;
        public int MaxMillisecondsBetweenBatches { get; set; } = 1000;
        public int MaxDegreeOfParallelism = Environment.ProcessorCount * 4;
        public int MaxQueueSize = 100000;
        public Func<int, int, int> SendThrottlingDelayFunc { get; set; } = (queueSize, maxBuffer) => (queueSize - maxBuffer) * 10;
    }

    public class EventHubAnalyticsClient : IAnalyticsClient
    {
        private EventHubClient _client;
        private ConcurrentQueue<EventData> _queue = new ConcurrentQueue<EventData>();
        private ConcurrentDictionary<int, Task> _sendTasks = new ConcurrentDictionary<int, Task>();
        private Task _backgroundTask;
        private bool _isSending = false;
        private EventHubAnalyticsClientOptions _options;

        public EventHubAnalyticsClient(string connectionString, string eventHubPath, EventHubAnalyticsClientOptions options = null)
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(connectionString)
            {
                EntityPath = eventHubPath
            };
            _client = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            if (options == null)
            {
                options = new EventHubAnalyticsClientOptions();
            }

            _options = options;
        }

        public async Task FlushAsync()
        {
            if (_isSending)
            {
                Console.WriteLine("FlushAsync: Waiting for sending to finish");
                // Wait until all messages have been sent
                while (_isSending)
                {
                    await Task.Delay(10);
                }
            }

            // Wait until all send tasks are done
            Console.WriteLine($"FlushAsync: Waiting for {_sendTasks.Count} send tasks to finish");
            await Task.WhenAll(_sendTasks.Values);

            Console.WriteLine("FlushAsync: Waiting for tasks to remove themselves");
            while (_sendTasks.Count > 0)
            {
                await Task.Delay(10);
            }

            Console.WriteLine("All messages and tasks flushed!");
        }

        public async Task SendMessageAsync(ITypeVersionMessage msg, DateTime? dbgEnqueuedTimeUtc = null)
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
            var queueCount = _queue.Count;

            var data = new EventData(bytes);

            if (queueCount > _options.MaxQueueSize)
            {
                // Throttle sending/queuing in order not to exhaust memory, by default that means
                // "delay one millisecond for every message that's in the queue above what is max allowed".
                // Meaning the more messages we get in the queue we delay more, making an efficient
                // throttling. No throttling is done if we haven't filled the queue.
                Console.Write("?");
                var millisecondsDelay = _options.SendThrottlingDelayFunc(queueCount, _options.MaxQueueSize);
                if (millisecondsDelay < 1)
                    millisecondsDelay = 1;

                await Task.Delay(millisecondsDelay);
            }

            _queue.Enqueue(data);
            await EnsureIsSendingAsync();
        }

        private async Task EnsureIsSendingAsync()
        {
            // If there is no Task/Thread running and sending messages in the queue or the current task
            // has completed, canceled or faulted, then start a new. Fast and without need for locking
            // or using a semaphore at this time. Most of the time this method will return here.
            if (_isSending)
                return;

            var semaphore = new SemaphoreSlim(1, 1);

            // Wait for "lock/semaphore"
            await semaphore.WaitAsync();

            try
            {
                // Check again, but this time "inside the lock/semaphore", meaning we are sure no one else
                // are trying to create or restart the Task/Thread. This is needed since the semaphore could
                // have held one thread/task from going forward while another just created the new Task
                if (_isSending)
                    return;

                _isSending = true;
                _backgroundTask = Task.Run((Action)SendMessagesInQueue);
            }
            finally
            {
                semaphore.Release();
            }
        }

        private void SendMessagesInQueue()
        {
            Console.WriteLine("SendMessagesInQueue - Start");

            while (!_queue.IsEmpty)
            {
                var eventDatas = new List<EventData>();

                var sw = Stopwatch.StartNew();

                var i = 0;

                while (true)
                {
                    if (i > 0 && (i >= _options.MaxBatchSize ||               // reached max number of messages in batch
                        sw.ElapsedMilliseconds >= _options.MaxMillisecondsBetweenBatches))  // reached max time between batches
                        break;

                    if (_queue.TryDequeue(out var data))
                    {
                        // Add data to list of messages to send and continue with next
                        eventDatas.Add(data);
                        i++;
                        continue;
                    }

                    Thread.Sleep(100); // Don't exhaust the CPU, sleep for a while then try dequeue again.
                }

                while (_sendTasks.Count >= _options.MaxDegreeOfParallelism)
                {
                    // Throttle speed if we are using more concurrent send tasks than we want
                    Thread.Sleep(100);
                }


                var sendTask = _client.SendAsync(eventDatas);
                _sendTasks.TryAdd(sendTask.Id, sendTask);
                sendTask.ContinueWith((t) =>
                {
                    if (t.IsFaulted)
                    {
                        Console.WriteLine(t.Exception);
                    }

                    Console.Write("~");
                    _sendTasks.TryRemove(t.Id, out var task);
                });

                Console.WriteLine($"Queue size: {_queue.Count}, Tasks: {_sendTasks.Count}, Batch Size: {eventDatas.Count}");
            }

            Console.WriteLine("SendMessagesInQueue - End");

            _isSending = false;
        }
    }
}
