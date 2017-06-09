// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Xunit;
using System.Linq;
using System.Threading.Tasks;


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
        public void TestRoundToPreviousInterval()
        {
            DateTime dt1_ = _dt1.RoundToPreviousInterval(JobInterval.Quarterly);
            DateTime dt2_ = _dt2.RoundToPreviousInterval(JobInterval.Hourly);
            DateTime dt3_ = _dt3.RoundToPreviousInterval(JobInterval.Quarterly);
            DateTime dt4_ = _dt4.RoundToPreviousInterval(JobInterval.Quarterly);
            DateTime dt5_ = _dt5.RoundToPreviousInterval(JobInterval.HalfHour);
            DateTime dt6_ = _dt6.RoundToPreviousInterval(JobInterval.HalfHour);
            Assert.Equal(10, (_dt1 - dt1_).TotalMinutes);
            Assert.Equal(1, (_dt2 - dt2_).TotalMinutes);
            Assert.Equal(1, (_dt3 - dt3_).TotalMinutes);
            Assert.Equal(14, (_dt4 - dt4_).TotalMinutes);
            Assert.Equal(2, (_dt5 - dt5_).TotalMinutes);
            Assert.Equal(29, (_dt6 - dt6_).TotalMinutes);
        }

        //as per an initial scheduler algorithm discussion
        //leaving it here for reference
        /* some options - 
           OPTION A - empty blob storage
           current DT is 9/6, 5.01AM
           I give it a job 'run every day, 5 AM' with no start date. Then first time the job will run will be 10/6,
           shortly after 5AM (depending on the Task.Wait on the executor loop)

           OPTION B - empty blob storage
           current DT is 9/6, 4.59AM
           I give it a job 'run every day, 5 AM' with no start date. Then first time the job will run
           will be 9/6, meaning in a few minutes

           OPTION C - empty blob storage
           current DT is 9/6, 5.01AM
           I give it a job 'run every day, 5AM' with startDate 7/6. Then, these jobs will start *immediately*
           7/6 job
           8/6 job
           9/6 job

           and the 10/6 job will start after ~24hours 
           */
        [Fact]
        public async Task TestCalculateDailyExecutionsAsync()
        {
            //JobNeverExecuted refers to empty blob storage

            //current dt 9/6/2017 5:01
            //first requested execution is 9/6/2017 5:00 => 1 datetimes to execute
            RunOncePerDaySchedule schedule1 = new RunOncePerDaySchedule
                (JobNever.Executed, 5, 0,
                firstExecutionRequest: new DateTime(2017, 6, 9),
                timeProvider: CurrentUTCTime.Is(2017, 6, 9, 5, 1));
            var list = (await schedule1.GetPendingExecutionsAsync("")).ToList();
            Assert.Equal(1, list.Count());
            Assert.Equal(new DateTime(2017, 6, 9, 5, 0, 0), list[0]);
            //if we change the current dt to 9/6/2017 4:59 => 0 datetime
            schedule1 = new RunOncePerDaySchedule
                (JobNever.Executed, 5, 0,
                firstExecutionRequest: new DateTime(2017, 6, 9),
                timeProvider: CurrentUTCTime.Is(2017, 6, 9, 4, 59));
            list = (await schedule1.GetPendingExecutionsAsync("")).ToList();
            Assert.Equal(0, list.Count());
            //but if we change the current dt to 10/6/2017 5:01 => 2 datetime
            schedule1 = new RunOncePerDaySchedule
                (JobNever.Executed, 5, 0, firstExecutionRequest: new DateTime(2017, 6, 9),
                timeProvider: CurrentUTCTime.Is(2017, 6, 10, 5, 1));
            list = (await schedule1.GetPendingExecutionsAsync("")).ToList();
            Assert.Equal(2, list.Count());
            Assert.Equal(new DateTime(2017, 6, 9, 5, 0, 0), list[0]);
            Assert.Equal(new DateTime(2017, 6, 10, 5, 0, 0), list[1]);

            //current dt 9/6/2017 5:01
            //first requested execution is 7/6/2017 5:00 => 3 datetime to execute
            schedule1 = new RunOncePerDaySchedule
                (JobNever.Executed, 5, 0, firstExecutionRequest: new DateTime(2017, 6, 7),
                timeProvider: CurrentUTCTime.Is(2017, 6, 9, 5, 1));
            list = (await schedule1.GetPendingExecutionsAsync("")).ToList();
            Assert.Equal(3, (list.Count()));
            Assert.Equal(new DateTime(2017, 6, 7, 5, 0, 0), list[0]);
            Assert.Equal(new DateTime(2017, 6, 8, 5, 0, 0), list[1]);
            Assert.Equal(new DateTime(2017, 6, 9, 5, 0, 0), list[2]);


            //current dt 9/6/2017 5:01
            //job never executed
            //first requested execution is 5:00 => 0 datetimes to execute
            RunOncePerDaySchedule schedule2 = new RunOncePerDaySchedule
                (JobNever.Executed, 5, 0,
                timeProvider: CurrentUTCTime.Is(2017, 6, 9, 5, 1));
            var list2 = (await schedule2.GetPendingExecutionsAsync("")).ToList();
            Assert.Equal(0, list2.Count());
            //change "system" datetime to 10/6/2017, 5:05AM
            Utilities.ModifyMockSystemTime(schedule2, new DateTime(2017, 6, 10, 5, 5, 0));
            list2 = (await schedule2.GetPendingExecutionsAsync("")).ToList();
            Assert.Equal(1, list2.Count());
            Assert.Equal(new DateTime(2017, 6, 10, 5, 0, 0), list2[0]);
            //change "system" datetime to 12/6/2017, 5:05AM
            Utilities.ModifyMockSystemTime(schedule2, new DateTime(2017, 6, 12, 5, 5, 0));
            list2 = (await schedule2.GetPendingExecutionsAsync("")).ToList();
            Assert.Equal(3, list2.Count());
            Assert.Equal(new DateTime(2017, 6, 10, 5, 0, 0), list2[0]);
            Assert.Equal(new DateTime(2017, 6, 11, 5, 0, 0), list2[1]);
            Assert.Equal(new DateTime(2017, 6, 12, 5, 0, 0), list2[2]);

            //current dt 9/6/2017 5:01
            //job last executed on 9/6/2017, 5:00
            //first requested execution is 9/6/2017 5:00 => 0 datetimes to execute
            //remember that since job was executed before, the first requested execution will be ignored
            RunOncePerDaySchedule schedule3 = new RunOncePerDaySchedule
                (JobLastExecuted.On(2017, 6, 9, 5, 0), 5, 0,
                firstExecutionRequest: new DateTime(2017, 6, 9),
                timeProvider: CurrentUTCTime.Is(2017, 6, 9, 5, 1));
            var list3 = (await schedule3.GetPendingExecutionsAsync("")).ToList();
            Assert.Equal(0, list3.Count());

            //current dt 12/6/2017 5:01
            //job last executed on 9/6/2017, 5:00 => 3 datetimes to execute
            schedule3 = new RunOncePerDaySchedule
                (JobLastExecuted.On(2017, 6, 9, 5, 0), 5, 0,
                timeProvider: CurrentUTCTime.Is(2017, 6, 12, 5, 1));
            list3 = (await schedule3.GetPendingExecutionsAsync("")).ToList();
            Assert.Equal(3, list3.Count());
            Assert.Equal(new DateTime(2017, 6, 10, 5, 0, 0), list3[0]);
            Assert.Equal(new DateTime(2017, 6, 11, 5, 0, 0), list3[1]);
            Assert.Equal(new DateTime(2017, 6, 12, 5, 0, 0), list3[2]);

            //current dt 11/6/2017 4:59
            //job last executed on 9/6/2017, 5:00 => 1 datetimes to execute
            schedule3 = new RunOncePerDaySchedule
                (JobLastExecuted.On(2017, 6, 9, 5, 0), 5, 0,
                timeProvider: CurrentUTCTime.Is(2017, 6, 11, 4, 59));
            list3 = (await schedule3.GetPendingExecutionsAsync("")).ToList();
            Assert.Equal(1, list3.Count());
            Assert.Equal(new DateTime(2017, 6, 10, 5, 0, 0), list3[0]);


            //same as before, but with a firstExecutionRequest
            //which is ignored
            schedule3 = new RunOncePerDaySchedule
                (JobLastExecuted.On(2017, 6, 9, 5, 0), 5, 0,
                firstExecutionRequest: new DateTime(2017, 6, 9),
                timeProvider: CurrentUTCTime.Is(2017, 6, 11, 4, 59));
            list3 = (await schedule3.GetPendingExecutionsAsync("")).ToList();
            Assert.Equal(1, list3.Count());
            Assert.Equal(new DateTime(2017, 6, 10, 5, 0, 0), list3[0]);
        }


        [Fact]
        public void TestDetailedJobName()
        {
            //test the job names produced by the RunOncePerDaySchedule
            //these will be used as blob names on the blob storage
            var schedule = new RunOncePerDaySchedule(JobNever.Executed, 5, 1);
            var schedule2 = new RunOncePerDaySchedule(JobNever.Executed, 15, 18);
            var schedule3 = new RunOncePerDaySchedule(JobNever.Executed, 5, 11);

            Assert.Equal("job1_daily_05_01", schedule.GetDetailedJobName("job1"));
            Assert.Equal("azurejob_daily_15_18", schedule2.GetDetailedJobName("azurejob"));
            Assert.Equal("lala_daily_05_11", schedule3.GetDetailedJobName("lala"));
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
                _dt1.RoundToPreviousInterval(JobInterval.HalfHour), JobInterval.HalfHour);
            int int2 = DateTimeUtilities.CalculateTimePeriods(_dt3 -
                _dt1.RoundToPreviousInterval(JobInterval.HalfHour), JobInterval.HalfHour);
            int int3 = DateTimeUtilities.CalculateTimePeriods(_dt4 -
                _dt1.RoundToPreviousInterval(JobInterval.HalfHour), JobInterval.HalfHour);
            int int4 = DateTimeUtilities.CalculateTimePeriods(_dt5 -
                _dt1.RoundToPreviousInterval(JobInterval.Quarterly), JobInterval.Quarterly);
            int int5 = DateTimeUtilities.CalculateTimePeriods(_dt6 -
                _dt1.RoundToPreviousInterval(JobInterval.Quarterly), JobInterval.Quarterly);
            int int6 = DateTimeUtilities.CalculateTimePeriods(_dt4 -
                _dt1.RoundToPreviousInterval(JobInterval.Quarterly), JobInterval.Quarterly);
            int int7 = DateTimeUtilities.CalculateTimePeriods(_dt6 -
                _dt1.RoundToPreviousInterval(JobInterval.Hourly), JobInterval.Hourly);
            int int8 = DateTimeUtilities.CalculateTimePeriods(_dt5 -
                _dt1.RoundToPreviousInterval(JobInterval.Hourly), JobInterval.Hourly);
            int int9 = DateTimeUtilities.CalculateTimePeriods(_dt4 -
                _dt1.RoundToPreviousInterval(JobInterval.Hourly), JobInterval.Hourly);

            Assert.Equal(4, int1);
            Assert.Equal(27, int2);
            Assert.Equal(27, int3);
            Assert.Equal(60, int4);
            Assert.Equal(59, int5);
            Assert.Equal(52, int6);
            Assert.Equal(16, int7);
            Assert.Equal(16, int8);
            Assert.Equal(14, int9);
        }
    }
}

