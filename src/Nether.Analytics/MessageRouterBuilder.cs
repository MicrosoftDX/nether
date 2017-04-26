// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace Nether.Analytics
{
    public class MessageRouterBuilder : MessageRouterBuilder<Message>
    {

    }

    public class MessageRouterBuilder<ParsedMessageType> where ParsedMessageType : IKnownMessageType
    {
        private List<IMessageHandler<ParsedMessageType>> _messageHandlers = new List<IMessageHandler<ParsedMessageType>>();
        private List<MessagePipelineBuilder<ParsedMessageType>> _eventPipelineBuilders = new List<MessagePipelineBuilder<ParsedMessageType>>();
        private MessagePipelineBuilder<ParsedMessageType> _unhandledEventBuilder;

        public MessageRouterBuilder()
        {
        }

        public MessagePipelineBuilder<ParsedMessageType> Event(string eventName)
        {
            var builder = new MessagePipelineBuilder<ParsedMessageType>(eventName);
            _eventPipelineBuilders.Add(builder);

            return builder;
        }

        public MessageRouterBuilder<ParsedMessageType> AddMessageHandler(IMessageHandler<ParsedMessageType> messageHandler)
        {
            _messageHandlers.Add(messageHandler);

            return this;
        }

        public MessagePipelineBuilder<ParsedMessageType> UnhandledEvent()
        {
            _unhandledEventBuilder = new MessagePipelineBuilder<ParsedMessageType>(null);

            return _unhandledEventBuilder;
        }

        public MessageRouter<ParsedMessageType> Build()
        {
            var unhandledEventPipeline = _unhandledEventBuilder.Build(_messageHandlers);
            var eventPipelines = _eventPipelineBuilders.Select(p => p.Build(_messageHandlers)).ToDictionary(p => p.MessageType);

            return new MessageRouter<ParsedMessageType>(eventPipelines, unhandledEventPipeline);
        }
    }
}