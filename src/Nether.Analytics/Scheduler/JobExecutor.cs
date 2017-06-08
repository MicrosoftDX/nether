// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Nether.Analytics
{
    public class JobExecutor
    {
        private ISynchronizationProvider _synchronizationProvider;

        public JobExecutor(ISynchronizationProvider synchronizationProvider)
        {
            _synchronizationProvider = synchronizationProvider;
        }

        public async Task RunAsSingletonAsync(string jobName, JobScheduleBase schedule, Func<DateTime, Task> action)
        {
            //get pending executions
            var dateTimes = await schedule.GetPendingExecutionsAsync(jobName);
            //for all missed opportunities, execute action
            foreach (var dateTime in dateTimes)
            {
                //try to get a lease
                string leaseID = await _synchronizationProvider.AcquireLeaseAsync(jobName);

                Console.WriteLine($"Attempting to run job {jobName} for DateTime {dateTime.RoundToPreviousInterval(schedule.Interval)}");
                await action(dateTime); //execute requested action
                Console.WriteLine($"Finished running run job {jobName} for DateTime {dateTime.RoundToPreviousInterval(schedule.Interval)}");
                //notify the state provider
                await schedule.SetLastExecutionAsync(jobName, dateTime, leaseID);

                //notify sync provider that we do not longer need the lease/exclusive access
                await _synchronizationProvider.ReleaseLeaseAsync(jobName, leaseID);
            }
        }
    }
}
