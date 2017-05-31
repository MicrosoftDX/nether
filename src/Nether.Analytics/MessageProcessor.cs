// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether.Analytics.Parsers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    public class MessageProcessor<T>
    {
        private MessageProcessorInformation _info = new MessageProcessorInformation();
        private Func<MessageProcessorInformation, Task> _infoFuncAsync;
        private Timer _infoTimer;
        private SemaphoreSlim _timerSemaphore = new SemaphoreSlim(1, 1);

        public IMessageListener<T> Listener { get; set; }
        public IMessageParser<T> Parser { get; set; }
        public IMessageRouter Router { get; set; }

        //public long PerfMessagesPerSecond { get; private set; }

        public MessageProcessor(IMessageListener<T> listener, IMessageParser<T> parser, IMessageRouter router, Func<MessageProcessorInformation, Task> infoFuncAsync = null)
        {
            Listener = listener;
            Parser = parser;
            Router = router;

            if (infoFuncAsync != null)
            {
                _infoFuncAsync = infoFuncAsync;
                _infoTimer = new Timer(OnTimer, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            }
        }

        private async void OnTimer(object state)
        {
            // Use semaphore to make sure "_infoFuncAsync" is only called once at a time,
            // i.e. if _infoFuncAsync takes longer time to execute than the timer interval
            if (_timerSemaphore.Wait(0))
            {
                try
                {
                    _info.ResetInterval();

                    await _infoFuncAsync(_info);
                }
                finally
                {
                    _timerSemaphore.Release();
                }
            }
        }

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

        private async Task ProcessMessagesAsync(string partitionId, IEnumerable<T> unparsedMessages)
        {
            //Parallel.ForEach(unparsedMessages, async unparsedMessage =>
            //{
            //    var parsedMessage = await Parser.ParseMessageAsync(unparsedMessage);
            //    if (parsedMessage == null)//incoming message was corrupt
            //        return;//go on with the next message

            //    await Router.RouteMessageAsync(parsedMessage);

            //    _info.AddMessageCount(1);
            //});

            int i = 0;

            foreach (var unparsedMessage in unparsedMessages)
            {
                var parsedMessage = await Parser.ParseMessageAsync(unparsedMessage);
                if (parsedMessage == null)//incoming message was corrupt
                    continue;//go on with the next message

                await Router.RouteMessageAsync(partitionId, parsedMessage);
                i++;
            }

            await Router.FlushAsync(partitionId);

            _info.AddMessageCount(i);
        }
    }
}
