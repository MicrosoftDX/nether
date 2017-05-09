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
        public MessageProcessor(IMessageListener<T> listner, IMessageParser<T> parser, IMessageRouter router)
        {
            Listener = listner;
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
            await Listener.StartAsync(ProcessMessagesAsync);
            await cancellationToken.WhenCanceled();
        }

        private async Task ProcessMessagesAsync(IEnumerable<T> unparsedMessages)
        {
            SetupCultureInfo();

            //TODO: Run this loop in parallel
            foreach (var unparsedMessage in unparsedMessages)
            {
                var parsedMessage = Parser.ParseMessage(unparsedMessage);
                await Router.RouteMessageAsync(parsedMessage);
            }
        }

        private static void SetupCultureInfo()
        {
            // Create a unique culture for all threads inside the MessageProcessor
            // in order to make sure input and output of serialization and de-
            // serialization all look according to expected results
            // Nether uses a modified en-US culture
            var netherCultureInfo = new CultureInfo("en-US");
            netherCultureInfo.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            netherCultureInfo.DateTimeFormat.LongTimePattern = "HH:mm:ss";

            CultureInfo.DefaultThreadCurrentCulture = netherCultureInfo;
        }
    }
}
