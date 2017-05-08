// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task RouteMessageAsync(IMessage msg)
        {
            if (_eventPipelines.TryGetValue(msg.MessageType, out var pipeline))
            {
                await pipeline.ProcessMessageAsync(msg);
            }
            else
            {
                await _unhandledEventPipeline?.ProcessMessageAsync(msg);
            }
        }
    }
}