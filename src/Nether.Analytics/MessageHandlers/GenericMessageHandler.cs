// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    public class GenericMessageHandler : IMessageHandler
    {
        private Func<Message, string, int, Task<MessageHandlerResults>> _asyncFunc;

        /// <summary>
        /// Instantiates new Generic Message Handler
        /// </summary>
        /// <param name="asyncFunc">Function (Message msg, string pipelineName, int idx) that handles messages</param>
        public GenericMessageHandler(Func<Message, string, int, Task<MessageHandlerResults>> asyncFunc)
        {
            _asyncFunc = asyncFunc;
        }

        public async Task<MessageHandlerResults> ProcessMessageAsync(Message msg, string pipelineName, int idx)
        {
            return await _asyncFunc(msg, pipelineName, idx);
        }
    }
}