// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    public class NullMessageHandler : IMessageHandler
    {
        //private object[] _outputManagers;

        //public NullMessageHandler(params object[] outputManagers)
        //{
        //    _outputManagers = outputManagers;
        //}

        public Task<MessageHandlerResluts> ProcessMessageAsync(Message msg, string pipelineName, int idx)
        {
            // Don't do anything since this is a NullMessageHandler

            return Task.FromResult(MessageHandlerResluts.Success);
        }
    }
}