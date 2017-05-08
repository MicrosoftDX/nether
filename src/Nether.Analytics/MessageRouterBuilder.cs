// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nether.Analytics
{
    public class MessageRouterBuilder
    {
        private List<IMessageHandler> _messageHandlers = new List<IMessageHandler>();
        private List<MessagePipelineBuilder> _eventPipelineBuilders = new List<MessagePipelineBuilder>();
        private MessagePipelineBuilder _unhandledEventBuilder;

        public MessageRouterBuilder()
        {
        }

        public MessagePipelineBuilder Event(string eventName)
        {
            var builder = new MessagePipelineBuilder(eventName);
            _eventPipelineBuilders.Add(builder);

            return builder;
        }

        public MessageRouterBuilder AddMessageHandler(IMessageHandler messageHandler)
        {
            _messageHandlers.Add(messageHandler);

            return this;
        }

        public MessagePipelineBuilder UnhandledEvent()
        {
            _unhandledEventBuilder = new MessagePipelineBuilder(null);

            return _unhandledEventBuilder;
        }

        public MessageRouter Build()
        {
            var unhandledEventPipeline = _unhandledEventBuilder.Build(_messageHandlers);
            var eventPipelines = _eventPipelineBuilders.Select(p => p.Build(_messageHandlers)).ToDictionary(p => p.MessageType);

            return new MessageRouter(eventPipelines, unhandledEventPipeline);
        }

        public object XXXPipeline(string v)
        {
            throw new NotImplementedException();
        }
    }
}