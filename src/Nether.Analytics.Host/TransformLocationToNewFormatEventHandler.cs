// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Nether.Analytics.Host
{
    public class TransformLocationToNewFormatEventHandler : IMessageHandler
    {
        public Task<MessageHandlerResluts> ProcessMessageAsync(Message msg, string pipelineName, int idx)
        {
            throw new NotImplementedException();
        }
    }
}