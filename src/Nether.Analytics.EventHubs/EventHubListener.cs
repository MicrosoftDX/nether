// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;



namespace Nether.Analytics.EventHubs
{
    public class EventHubsListener : IEventProcessor, IMessageListener<EventHubListenerMessage>
    {
        private readonly EventProcessorHost _host;
        private Func<string, IEnumerable<EventHubListenerMessage>, Task> _messageHandlerAsync;
        private EventProcessorOptions _processorOptions;

        public EventHubsListener(EventProcessorHostOptions hostOptions, EventProcessorOptions processorOptions = null)
        {
            #region Assert arguments are provided
            if (string.IsNullOrWhiteSpace(hostOptions.EventHubPath))
                throw new ArgumentException("EventHubPath needs to be provided");

            if (string.IsNullOrWhiteSpace(hostOptions.EventHubConnectionString))
                throw new ArgumentException("EventHubConnectionString needs to be provided");

            if (string.IsNullOrWhiteSpace(hostOptions.StorageConnectionString))
                throw new ArgumentException("StorageConnectionString needs to be provided");

            if (string.IsNullOrWhiteSpace(hostOptions.LeaseContainerName))
                throw new ArgumentException("LeaseContainerName needs to be provided");

            // If ConsumerGroupName is left null or empty, then use default ConsumerGroupName
            var consumerGroupName = string.IsNullOrWhiteSpace(hostOptions.ConsumerGroupName) ?
                PartitionReceiver.DefaultConsumerGroupName : hostOptions.ConsumerGroupName;
            #endregion

            _host = new EventProcessorHost(
                hostOptions.EventHubPath,
                consumerGroupName,
                hostOptions.EventHubConnectionString,
                hostOptions.StorageConnectionString,
                hostOptions.LeaseContainerName);

            _processorOptions = processorOptions ?? new EventProcessorOptions();
        }

        public async Task StartAsync(Func<string, IEnumerable<EventHubListenerMessage>, Task> messageHandlerAsync)
        {
            _messageHandlerAsync = messageHandlerAsync;
            // Register this object as the processor of incomming EventHubMessages by using
            // a custom EventHubProcessorFactory

            await _host.RegisterEventProcessorFactoryAsync(new EventHubProcessorFactory(this), _processorOptions);
        }

        public async Task StopAsync()
        {
            await _host.UnregisterEventProcessorAsync();
            _messageHandlerAsync = null;
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            var eventHubListenerMessages = new List<EventHubListenerMessage>();
            var dequeuedTime = DateTime.UtcNow;

            foreach (var msg in messages)
            {
                eventHubListenerMessages.Add(new EventHubListenerMessage
                {
                    PartitionId = context.PartitionId,
                    SequenceNumber = msg.SystemProperties.SequenceNumber,
                    Body = msg.Body,
                    EnqueuedTime = msg.SystemProperties.EnqueuedTimeUtc,
                    DequeuedTime = dequeuedTime
                });
            }

            await _messageHandlerAsync(context.PartitionId, eventHubListenerMessages);

            await context.CheckpointAsync();
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            //TODO: Fix this
            Console.WriteLine(error.ToString());
            //throw new NotImplementedException();
            return Task.CompletedTask;
        }


        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine($"EventHubProcessor.CloseAsync Owner:{context.Owner}, PartitionId:{context.PartitionId}");

            return Task.CompletedTask;
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine($"EventHubProcessor.OpenAsync Owner:{context.Owner}, PartitionId:{context.PartitionId}");

            return Task.CompletedTask;
        }


        /// <summary>
        /// Private factory class to make sure we use the current instance of EventHubProcessor as the
        /// instance for the EventProcessorHost
        /// </summary>
        private class EventHubProcessorFactory : IEventProcessorFactory
        {
            private IEventProcessor _processor;

            public EventHubProcessorFactory(IEventProcessor processor)
            {
                _processor = processor;
            }

            public IEventProcessor CreateEventProcessor(PartitionContext context)
            {
                return _processor;
            }
        }
    }
}