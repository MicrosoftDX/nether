// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether.Analytics.Parsers;
using System;
using System.Threading.Tasks;

namespace Nether.Analytics.Host
{
    public class GamerInfoEnricher : IMessageHandler
    {
        public Task<MessageHandlerResluts> ProcessMessageAsync(IMessage msg)
        {
            msg.Properties.Add("Greeting", "Event was enriched");

            return Task.FromResult(MessageHandlerResluts.Success);
        }
    }
}