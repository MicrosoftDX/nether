// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    /// <summary>
    /// Message Handler Extention Methods that helps using and creating Message Handlers
    /// </summary>
    public static class MessageHandlerEx
    {
        public static MessagePipelineBuilder AddHandler(this MessagePipelineBuilder builder, Func<Message, string, int, Task<MessageHandlerResluts>> asyncFunc)
        {
            return builder.AddHandler(new GenericMessageHandler(asyncFunc));
        }

        public static MessagePipelineBuilder AddHandler(this MessagePipelineBuilder builder, Func<Message, string, Task<MessageHandlerResluts>> asyncFunc)
        {
            return builder.AddHandler(new GenericMessageHandler((msg, pipelineName, idx) => asyncFunc(msg, pipelineName)));
        }

        public static MessagePipelineBuilder AddHandler(this MessagePipelineBuilder builder, Func<Message, Task<MessageHandlerResluts>> asyncFunc)
        {
            return builder.AddHandler(new GenericMessageHandler((msg, pipelineName, idx) => asyncFunc(msg)));
        }

        public static MessagePipelineBuilder AddHandler(this MessagePipelineBuilder builder, Action<Message, string, int> action)
        {
            return builder.AddHandler(new GenericMessageHandler((msg, pipelineName, idx) =>
            {
                action(msg, pipelineName, idx);

                return Task.FromResult(MessageHandlerResluts.Success);
            }));
        }

        public static MessagePipelineBuilder AddHandler(this MessagePipelineBuilder builder, Action<Message, string> action)
        {
            return AddHandler(builder, (msg, pipelineName, idx) => action(msg, pipelineName));
        }

        public static MessagePipelineBuilder AddHandler(this MessagePipelineBuilder builder, Action<Message> action)
        {
            return AddHandler(builder, (msg, pipelineName, idx) => action(msg));
        }
    }
}