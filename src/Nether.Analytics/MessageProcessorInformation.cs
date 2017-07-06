// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Threading;

namespace Nether.Ingest
{
    public class MessageProcessorInformation
    {
        private long _messagesTotal;
        private long _messagesInInterval;
        private Stopwatch _totalTime = Stopwatch.StartNew();
        private Stopwatch _timeInInterval = Stopwatch.StartNew();

        public long MessagesPerSecond { get; internal set; }
        public long TotalMessages => _messagesTotal;
        public TimeSpan TotalTime => _totalTime.Elapsed;

        public void AddMessageCount(long value)
        {
            // Using Interlocked increment since values could be increased from several threads at once
            Interlocked.Add(ref _messagesTotal, value);
            Interlocked.Add(ref _messagesInInterval, value);
        }

        public void ResetInterval()
        {
            var messagesInInterval = Interlocked.Read(ref _messagesInInterval);

            // Freeze Messages Per Second during the interval by calculating and storing it
            if (_timeInInterval.Elapsed.TotalSeconds == 0)
                MessagesPerSecond = 0;
            else
                MessagesPerSecond = (long)Math.Ceiling(messagesInInterval / _timeInInterval.Elapsed.TotalSeconds);

            Interlocked.Add(ref _messagesInInterval, -messagesInInterval);
            _timeInInterval.Restart();
        }
    }
}
