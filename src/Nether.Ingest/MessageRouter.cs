// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Nether.Ingest
{
    public class MessageRouter : IMessageRouter
    {
        private Dictionary<string, List<MessagePipeline>> _routingDictionary = new Dictionary<string, List<MessagePipeline>>();
        private MessagePipeline _unhandledEventPipeline;

        public MessageRouter(List<MessagePipeline> messagePipelines, MessagePipeline unhandledEventPipeline)
        {
            foreach (var pipeline in messagePipelines)
            {
                foreach (var messageType in pipeline.HandledMessageTypes)
                {
                    if (_routingDictionary.TryGetValue(messageType, out var existingPipelinesForMessageType))
                    {
                        existingPipelinesForMessageType.Add(pipeline);
                    }
                    else
                    {
                        var newPipelinesForMessageType = new List<MessagePipeline>();
                        newPipelinesForMessageType.Add(pipeline);
                        _routingDictionary.Add(messageType, newPipelinesForMessageType);
                    }
                }
            }
            _unhandledEventPipeline = unhandledEventPipeline;
        }

        public async Task RouteMessageAsync(string partitionId, Message msg)
        {
            if (_routingDictionary.TryGetValue(msg.VersionedMessageType, out var pipelines))
            {
                //TODO: Run loop in parallel
                foreach (var pipeline in pipelines)
                {
                    await pipeline.ProcessMessageAsync(partitionId, msg);
                }
            }
            else if (_unhandledEventPipeline != null)
            {
                //default pipeline
                await _unhandledEventPipeline?.ProcessMessageAsync(partitionId, msg);
            }
        }

        public async Task FlushAsync(string partitionId)
        {
            foreach (var pipelines in _routingDictionary.Values)
            {
                foreach (var pipeline in pipelines)
                {
                    await pipeline.FlushAsync(partitionId);
                }
            }
        }
    }
}