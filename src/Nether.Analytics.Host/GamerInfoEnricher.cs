// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.



using Nether.Analytics.Parsers;
using System;

namespace Nether.Analytics.Host
{
    public class GamerInfoEnricher : IMessageHandler
    {
        public MessageHandlerResluts ProcessMessage(IMessage msg)
        {
            msg.Properties.Add("Greeting", "Event was enriched");

            return MessageHandlerResluts.Success;
        }
    }
}