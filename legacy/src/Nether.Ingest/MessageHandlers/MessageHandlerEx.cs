// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Nether.Ingest
{
    /// <summary>
    /// Message Handler Extention Methods that helps using and creating Message Handlers
    /// </summary>
    public static class MessageHandlerEx
    {
        public static MessagePipelineBuilder AddHandler(this MessagePipelineBuilder builder, Func<Message, string, int, Task<MessageHandlerResults>> asyncFunc)
        {
            return builder.AddHandler(new GenericMessageHandler(asyncFunc));
        }

        public static MessagePipelineBuilder AddHandler(this MessagePipelineBuilder builder, Func<Message, string, Task<MessageHandlerResults>> asyncFunc)
        {
            return builder.AddHandler(new GenericMessageHandler((msg, pipelineName, index) => asyncFunc(msg, pipelineName)));
        }

        public static MessagePipelineBuilder AddHandler(this MessagePipelineBuilder builder, Func<Message, Task<MessageHandlerResults>> asyncFunc)
        {
            return builder.AddHandler(new GenericMessageHandler((msg, pipelineName, index) => asyncFunc(msg)));
        }

        public static MessagePipelineBuilder AddHandler(this MessagePipelineBuilder builder, Action<Message, string, int> action)
        {
            return builder.AddHandler(new GenericMessageHandler((msg, pipelineName, index) =>
            {
                action(msg, pipelineName, index);

                return Task.FromResult(MessageHandlerResults.Success);
            }));
        }

        public static MessagePipelineBuilder AddHandler(this MessagePipelineBuilder builder, Action<Message, string> action)
        {
            return AddHandler(builder, (msg, pipelineName, index) => action(msg, pipelineName));
        }

        public static MessagePipelineBuilder AddHandler(this MessagePipelineBuilder builder, Action<Message> action)
        {
            return AddHandler(builder, (msg, pipelineName, index) => action(msg));
        }
    }
}