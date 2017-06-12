// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Nether.Analytics
{
    public class IntervalSchedule : IJobScheduleV2
    {
        private TimeSpan _interval;

        public IntervalSchedule(TimeSpan interval)
        {
            _interval = interval;
        }

        public DateTime GetNextExcecutionTime(DateTime lastExecutionTime)
        {
            return lastExecutionTime + _interval;
        }
    }
}