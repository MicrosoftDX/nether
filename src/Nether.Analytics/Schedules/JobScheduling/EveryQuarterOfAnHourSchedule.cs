// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Nether.Analytics
{
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
}