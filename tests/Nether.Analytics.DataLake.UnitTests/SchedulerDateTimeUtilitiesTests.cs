// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Xunit;
using System.Linq;

namespace Nether.Analytics.UnitTests
{
    public class SchedulerDateTimeUtilitiesTests
    {
        [Fact]
        public void TestTimeStringParsing()
        {
            DateTime dta = DateTimeUtilities.FromYMDHMString("20170521-2132");
            Assert.Equal(new DateTime(2017, 5, 21, 21, 32, 00), dta);

            string dtb = DateTimeUtilities.ToYMDHMSString(new DateTime(2017, 3, 12, 10, 23, 00));
            Assert.Equal("20170312-1023", dtb);
        }

        [Fact]
        public void TestGetFirstDateTimeToStartExecutionSlot()
        {
            DateTime dt1_ = DateTimeUtilities.RoundToPreviousInterval(_dt1, JobInterval.Quarterly);
            DateTime dt2_ = DateTimeUtilities.RoundToPreviousInterval(_dt2, JobInterval.Hourly);
            DateTime dt3_ = DateTimeUtilities.RoundToPreviousInterval(_dt3, JobInterval.Quarterly);
            DateTime dt4_ = DateTimeUtilities.RoundToPreviousInterval(_dt4, JobInterval.Quarterly);
            DateTime dt5_ = DateTimeUtilities.RoundToPreviousInterval(_dt5, JobInterval.HalfHour);
            DateTime dt6_ = DateTimeUtilities.RoundToPreviousInterval(_dt6, JobInterval.HalfHour);
            Assert.Equal(10, (_dt1 - dt1_).TotalMinutes);
            Assert.Equal(1, (_dt2 - dt2_).TotalMinutes);
            Assert.Equal(1, (_dt3 - dt3_).TotalMinutes);
            Assert.Equal(14, (_dt4 - dt4_).TotalMinutes);
            Assert.Equal(2, (_dt5 - dt5_).TotalMinutes);
            Assert.Equal(29, (_dt6 - dt6_).TotalMinutes);
        }


        [Fact]
        public void TestCalculatePendingExecutions()
        {
            DateTime lastExecutionTimeHourly = new DateTime(2017, 5, 4, 12, 00, 00);
            DateTime lastExecutionTimeHalfHour = new DateTime(2017, 5, 4, 12, 30, 00);
            DateTime lastExecutionTimeQuarterly = new DateTime(2017, 5, 5, 9, 45, 00);

            var dtlistHourlyTest = DateTimeUtilities.GetIntervalsBetween(_dt1, lastExecutionTimeHourly, JobInterval.Hourly).ToList();
            Assert.Equal(11, dtlistHourlyTest.Count());
            for (int i = 1; i <= 11; i++)
            {
                Assert.Equal(lastExecutionTimeHourly.AddMinutes(i * JobInterval.Hourly.ToMinutes()), dtlistHourlyTest[i - 1]);
            }
            var dtlistHalfHourTest = DateTimeUtilities.GetIntervalsBetween(_dt2, lastExecutionTimeHalfHour, JobInterval.HalfHour).ToList();
            Assert.Equal(25, dtlistHalfHourTest.Count());
            for (int i = 1; i <= 25; i++)
            {
                Assert.Equal(lastExecutionTimeHalfHour.AddMinutes(i * JobInterval.HalfHour.ToMinutes()), dtlistHalfHourTest[i - 1]);
            }
            var dtlistQuarterlyTest = DateTimeUtilities.GetIntervalsBetween(_dt3, lastExecutionTimeQuarterly, JobInterval.Quarterly).ToList();
            Assert.Equal(12, dtlistQuarterlyTest.Count());
            for (int i = 1; i <= 12; i++)
            {
                Assert.Equal(lastExecutionTimeQuarterly.AddMinutes(i * JobInterval.Quarterly.ToMinutes()), dtlistQuarterlyTest[i - 1]);
            }
        }

        private readonly DateTime _dt1 = new DateTime(2017, 5, 4, 23, 55, 00);
        private readonly DateTime _dt2 = new DateTime(2017, 5, 5, 01, 01, 00);
        private readonly DateTime _dt3 = new DateTime(2017, 5, 5, 12, 46, 00);
        private readonly DateTime _dt4 = new DateTime(2017, 5, 5, 12, 44, 00);
        private readonly DateTime _dt5 = new DateTime(2017, 5, 5, 14, 32, 00);
        private readonly DateTime _dt6 = new DateTime(2017, 5, 5, 14, 29, 00);

        [Fact]
        public void TestCalculateTimePeriods()
        {
            int int1 = DateTimeUtilities.CalculateTimePeriods(_dt2 -
                DateTimeUtilities.RoundToPreviousInterval(
                    _dt1, JobInterval.HalfHour), JobInterval.HalfHour);
            int int2 = DateTimeUtilities.CalculateTimePeriods(_dt3 -
                DateTimeUtilities.RoundToPreviousInterval(
                    _dt1, JobInterval.HalfHour), JobInterval.HalfHour);
            int int3 = DateTimeUtilities.CalculateTimePeriods(_dt4 -
                DateTimeUtilities.RoundToPreviousInterval(
                    _dt1, JobInterval.HalfHour), JobInterval.HalfHour);
            int int4 = DateTimeUtilities.CalculateTimePeriods(_dt5 -
                DateTimeUtilities.RoundToPreviousInterval(
                    _dt1, JobInterval.Quarterly), JobInterval.Quarterly);
            int int5 = DateTimeUtilities.CalculateTimePeriods(_dt6 -
                DateTimeUtilities.RoundToPreviousInterval(
                    _dt1, JobInterval.Quarterly), JobInterval.Quarterly);
            int int6 = DateTimeUtilities.CalculateTimePeriods(_dt4 -
                DateTimeUtilities.RoundToPreviousInterval(
                    _dt1, JobInterval.Quarterly), JobInterval.Quarterly);
            int int7 = DateTimeUtilities.CalculateTimePeriods(_dt6 -
                DateTimeUtilities.RoundToPreviousInterval(
                    _dt1, JobInterval.Hourly), JobInterval.Hourly);
            int int8 = DateTimeUtilities.CalculateTimePeriods(_dt5 -
                DateTimeUtilities.RoundToPreviousInterval(
                    _dt1, JobInterval.Hourly), JobInterval.Hourly);
            int int9 = DateTimeUtilities.CalculateTimePeriods(_dt4 -
                DateTimeUtilities.RoundToPreviousInterval(
                    _dt1, JobInterval.Hourly), JobInterval.Hourly);


            Assert.Equal(3, int1);
            Assert.Equal(26, int2);
            Assert.Equal(26, int3);
            Assert.Equal(59, int4);
            Assert.Equal(58, int5);
            Assert.Equal(51, int6);
            Assert.Equal(15, int7);
            Assert.Equal(15, int8);
            Assert.Equal(13, int9);
        }
    }
}

