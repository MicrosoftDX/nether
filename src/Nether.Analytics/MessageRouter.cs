// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Nether.Analytics
{
    public class MessageRouter<ParsedMessageType> : IMessageRouter<ParsedMessageType> where ParsedMessageType : IKnownMessageType
    {
        private Dictionary<string, MessagePipeline<ParsedMessageType>> _eventPipelines;
        private MessagePipeline<ParsedMessageType> _unhandledEventPipeline;

        public MessageRouter(Dictionary<string, MessagePipeline<ParsedMessageType>> eventPipelines, MessagePipeline<ParsedMessageType> unhandledEventPipeline)
        {
            _eventPipelines = eventPipelines;
            _unhandledEventPipeline = unhandledEventPipeline;
        }

        public void RouteMessage(ParsedMessageType msg)
        {
            if (_eventPipelines.TryGetValue(msg.MessageType, out var pipeline))
            {
                pipeline.ProcessMessage(msg);
            }
            else
            {
                _unhandledEventPipeline?.ProcessMessage(msg);
            }
        }
    }
}