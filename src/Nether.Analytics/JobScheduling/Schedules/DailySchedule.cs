// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Nether.Analytics
{
    public class DailySchedule : IJobScheduleV2
    {
        private TimeSpan _atTime;

        /// <summary>
        /// Initializes a DailySchedule at 00:00:00 (midnight)
        /// </summary>
        public DailySchedule() : this(TimeSpan.Zero)
        {
        }

        /// <summary>
        /// Initializes a DailySchedule at the time specified by atTime.
        /// </summary>
        /// <param name="atTime">The time(span past midnight) that the schedule should be executed.</param>
        public DailySchedule(TimeSpan atTime)
        {
            _atTime = atTime;
        }

        public DailySchedule(int hours, int minutes, int seconds) : this(new TimeSpan(hours, minutes, seconds))
        { }

        public DateTime GetNextExecutionTime(DateTime lastExecutionTime)
        {
            var nextTime = new DateTime(
                lastExecutionTime.Year,
                lastExecutionTime.Month,
                lastExecutionTime.Day);

            nextTime += _atTime;

            if (nextTime <= lastExecutionTime)
                nextTime += TimeSpan.FromDays(1);

            return nextTime;
        }
    }
}