// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Nether.Ingest
{
    public class EveryHourOnTheHourSchedule : IJobScheduleV2
    {
        public DateTime GetNextExecutionTime(DateTime lastExecutionTime)
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
}