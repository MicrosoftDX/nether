// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Nether.Analytics
{
    public class WeeklySchedule : IJobScheduleV2
    {
        private TimeSpan _atTime;
        private HashSet<DayOfWeek> _daysOfWeek;

        public WeeklySchedule(TimeSpan atTime, params DayOfWeek[] daysOfWeek)
        {
            _atTime = atTime;

            if (daysOfWeek.Length == 0)
                throw new ArgumentException("Schedule needs to have at least one day of week to execute on");

            _daysOfWeek = new HashSet<DayOfWeek>(daysOfWeek);
        }

        public DateTime GetNextExecutionTime(DateTime lastExecutionTime)
        {
            var nextTime = new DateTime(
                lastExecutionTime.Year,
                lastExecutionTime.Month,
                lastExecutionTime.Day);

            nextTime += _atTime;

            do
            {
                //add another day
                nextTime += TimeSpan.FromDays(1);
            } while (!_daysOfWeek.Contains(nextTime.DayOfWeek));

            //while (nextTime <= lastExecutionTime && !_daysOfWeek.Contains(nextTime.DayOfWeek))
            //{
            //    nextTime += TimeSpan.FromDays(1);
            //}

            return nextTime;
        }
    }
}