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
                throw new ArgumentException("Schedule need to have at least one day of week to execute on");

            _daysOfWeek = new HashSet<DayOfWeek>(daysOfWeek);
        }

        public DateTime GetNextExcecutionTime(DateTime lastExecutionTime)
        {
            var nextTime = new DateTime(
                lastExecutionTime.Year,
                lastExecutionTime.Month,
                lastExecutionTime.Day);

            nextTime += _atTime;

            while (nextTime <= lastExecutionTime && !_daysOfWeek.Contains(nextTime.DayOfWeek))
            {
                nextTime += TimeSpan.FromDays(1);
            }

            return nextTime;
        }
    }

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

        public DateTime GetNextExcecutionTime(DateTime lastExecutionTime)
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

    public class EveryHourOnTheHourSchedule : IJobScheduleV2
    {
        public DateTime GetNextExcecutionTime(DateTime lastExecutionTime)
        {
            var nextTime = new DateTime(
                lastExecutionTime.Year,
                lastExecutionTime.Month,
                lastExecutionTime.Day,
                lastExecutionTime.Hour,
                0,
                0);

            return nextTime.AddHours(1);
        }
    }

    public class EveryQuarterOfAnHourSchedule : IJobScheduleV2
    {
        public DateTime GetNextExcecutionTime(DateTime lastExecutionTime)
        {
            var nextTime = new DateTime(
                lastExecutionTime.Year,
                lastExecutionTime.Month,
                lastExecutionTime.Day,
                lastExecutionTime.Hour,
                0,
                0);

            while (nextTime <= lastExecutionTime)
            {
                nextTime += TimeSpan.FromMinutes(15);
            }

            return nextTime;
        }
    }

    public class EveryMinuteOnTheMinuteSchedule : IJobScheduleV2
    {
        public DateTime GetNextExcecutionTime(DateTime lastExecutionTime)
        {
            var nextTime = new DateTime(
                lastExecutionTime.Year,
                lastExecutionTime.Month,
                lastExecutionTime.Day,
                lastExecutionTime.Hour,
                lastExecutionTime.Minute,
                0);

            return nextTime.AddMinutes(1);
        }
    }

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