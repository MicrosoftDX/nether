// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nether.Analytics
{
    public class MessageRouterBuilder
    {
        //private List<IMessageHandler> _messageHandlers = new List<IMessageHandler>();
        private List<MessagePipelineBuilder> _messagePipelineBuilders = new List<MessagePipelineBuilder>();
        private MessagePipelineBuilder _unhandledPipelineBuilder;

        public MessageRouterBuilder()
        {
        }

        public MessagePipelineBuilder Pipeline(string pipelineName)
        {
            var builder = new MessagePipelineBuilder(pipelineName);
            _messagePipelineBuilders.Add(builder);

            return builder;
        }

        public MessagePipelineBuilder DefaultPipeline
        {
            get
            {
                if (_unhandledPipelineBuilder == null)
                    _unhandledPipelineBuilder = new MessagePipelineBuilder("__DEFAULT__PIPELINE__");
                return _unhandledPipelineBuilder;
            }
        }

        //public MessageRouterBuilder AddMessageHandler(IMessageHandler messageHandler)
        //{
        //    _messageHandlers.Add(messageHandler);

        //    return this;
        //}

        //public MessagePipelineBuilder UnhandledEvent()
        //{
        //    _unhandledEventBuilder = new MessagePipelineBuilder(null);

        //    return _unhandledEventBuilder;
        //}

        public MessageRouter Build()
        {
            //var unhandledEventPipeline = _unhandledEventBuilder.Build(_messageHandlers);

            //assign the unhandled event handler to all message pipeline builders

            var eventPipelines = _messagePipelineBuilders.Select(p => p.Build()).ToList();

            var unhandledEventPipeline = _unhandledPipelineBuilder?.Build();
            return new MessageRouter(eventPipelines, unhandledEventPipeline);
        }
    }
}