// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Nether.Analytics
{
    public class MessageRouter : IMessageRouter
    {
        private Dictionary<string, MessagePipeline> _eventPipelines;
        private MessagePipeline _unhandledEventPipeline;

        public MessageRouter(Dictionary<string, MessagePipeline> eventPipelines, MessagePipeline unhandledEventPipeline)
        {
            _eventPipelines = eventPipelines;
            _unhandledEventPipeline = unhandledEventPipeline;
        }

        public void RouteMessage(IMessage msg)
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