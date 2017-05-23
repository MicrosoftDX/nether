// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether.Analytics.Parsers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    public class MessageProcessor<T>
    {
        public MessageProcessor(IMessageListener<T> listener, IMessageParser<T> parser, IMessageRouter router)
        {
            Listener = listener;
            Parser = parser;
            Router = router;
        }

        public IMessageListener<T> Listener { get; set; }
        public IMessageParser<T> Parser { get; set; }
        public IMessageRouter Router { get; set; }

        public async Task ProcessAndBlockAsync()
        {
            var cancellationToken = CancellationToken.None;

            await ProcessAndBlockAsync(cancellationToken);
        }

        public async Task ProcessAndBlockAsync(CancellationToken cancellationToken)
        {
            CultureInfoEx.SetupNetherCultureInfo();
            await Listener.StartAsync(ProcessMessagesAsync);
            await cancellationToken.WhenCanceled();
        }

        private async Task ProcessMessagesAsync(IEnumerable<T> unparsedMessages)
        {
            //TODO: Run this loop in parallel
            foreach (var unparsedMessage in unparsedMessages)
            {
                var parsedMessage = await Parser.ParseMessageAsync(unparsedMessage);
                if (parsedMessage == null)//incoming message was corrupt
                    continue;//go on with the next message
                await Router.RouteMessageAsync(parsedMessage);
            }
        }
    }
}
