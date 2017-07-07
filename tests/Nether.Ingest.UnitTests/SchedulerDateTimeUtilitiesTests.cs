// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Xunit;

namespace Nether.Ingest.UnitTests
{
    public class SchedulerDateTimeUtilitiesTests
    {
        private readonly DateTime _dt1 = new DateTime(2017, 5, 4, 23, 55, 00);
        private readonly DateTime _dt2 = new DateTime(2017, 5, 5, 01, 00, 00);
        private readonly DateTime _dt3 = new DateTime(2017, 5, 5, 12, 46, 00);
        private readonly DateTime _dt4 = new DateTime(2017, 5, 5, 12, 45, 00);
        private readonly DateTime _dt5 = new DateTime(2017, 6, 14, 7, 5, 00);

        [Fact]
        public void TestSchedules()
        {
            var s1 = new DailySchedule(23, 55, 0);
            var dt = s1.GetNextExecutionTime(_dt1);
            Assert.Equal(_dt1.AddDays(1), dt);

            var s2 = new EveryHourOnTheHourSchedule();
            dt = s2.GetNextExecutionTime(_dt2);
            Assert.Equal(_dt2.AddHours(1), dt);

            var s3 = new EveryMinuteOnTheMinuteSchedule();
            dt = s3.GetNextExecutionTime(_dt3);
            Assert.Equal(_dt3.AddMinutes(1), dt);

            var s4 = new EveryQuarterOfAnHourSchedule();
            dt = s4.GetNextExecutionTime(_dt4);
            Assert.Equal(_dt4.AddMinutes(15), dt);

            var ts = new TimeSpan(3, 4, 0);
            var s5 = new IntervalSchedule(ts);
            dt = s5.GetNextExecutionTime(_dt4);
            Assert.Equal(_dt4 + ts, dt);

            ts = new TimeSpan(7, 5, 0);
            var s6 = new WeeklySchedule(ts, DayOfWeek.Monday, DayOfWeek.Wednesday);
            //_dt5 is Wednesday (2017, 6, 14, 7, 5, 00)
            dt = s6.GetNextExecutionTime(_dt5);
            //dt should be next Monday, which is 19/6
            Assert.Equal(_dt5.AddDays(5), dt);
            dt = s6.GetNextExecutionTime(_dt5.AddDays(5));
            //when it's executed on Monday 19/6, next execution should be on Wednesday 21/6
            Assert.Equal(_dt5.AddDays(7), dt);
            dt = s6.GetNextExecutionTime(_dt5.AddDays(7));
            //and after execution on Wednesday 21/6, next one should be 26/6
            Assert.Equal(_dt5.AddDays(12), dt);
        }
    }
}

