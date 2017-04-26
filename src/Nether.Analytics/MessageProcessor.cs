// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether.Analytics.Parsers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    public class MessageProcessor<T> : MessageProcessor<T, Message>
    {
        public MessageProcessor(IMessageListener<T> listner, IMessageParser<T> parser, IMessageRouter<Message> router) : base(listner, parser, router)
        {
        }
    }

    public class MessageProcessor<TUnparsedMessage, TParsedMessage> where TParsedMessage : IKnownMessageType
    {
        public MessageProcessor(IMessageListener<TUnparsedMessage> listner, IMessageParser<TUnparsedMessage, TParsedMessage> parser, IMessageRouter<TParsedMessage> router)
        {
            Listener = listner;
            Parser = parser;
            Router = router;
        }

        public IMessageListener<TUnparsedMessage> Listener { get; set; }
        public IMessageParser<TUnparsedMessage, TParsedMessage> Parser { get; set; }
        public IMessageRouter<TParsedMessage> Router { get; set; }

        public async Task ProcessAndBlockAsync()
        {
            var cancellationToken = CancellationToken.None;

            await ProcessAndBlockAsync(cancellationToken);
        }

        public async Task ProcessAndBlockAsync(CancellationToken cancellationToken)
        {
            await Listener.StartAsync(ExecutePipeline);
            await cancellationToken.WhenCanceled();
        }

        private async Task ExecutePipeline(IEnumerable<TUnparsedMessage> unparsedMessages)
        {
            //TODO: Run this loop in parallel
            foreach (var unparsedMessage in unparsedMessages)
            {
                var parsedMessage = await Parser.ParseAsync(unparsedMessage);
                Router.RouteMessage(parsedMessage);
            }
        }
    }
}

public static class CancellationTokenEx
{
    public static Task WhenCanceled(this CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<bool>();
        cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
        return tcs.Task;
    }
}