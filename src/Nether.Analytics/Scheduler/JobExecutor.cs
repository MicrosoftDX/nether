// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    public class JobExecutor
    {
        private readonly ISynchronizationProvider _synchronizationProvider;

        public JobExecutor(ISynchronizationProvider synchronizationProvider)
        {
            _synchronizationProvider = synchronizationProvider;
        }

        public async Task RunAsSingletonAsync(string shortJobName, IJobSchedule schedule, Func<DateTime, Task> actionAsync)
        {
            //create the detailedJobName from the shortName
            string detailedJobName = schedule.GetDetailedJobName(shortJobName);
            //get pending executions from the schedule interface
            var dateTimes = await schedule.GetPendingExecutionsAsync(detailedJobName);
            //for all missed opportunities/pending executions, execute requested action
            foreach (var dateTime in dateTimes)
            {
                //try to get a lease
                string leaseID = await _synchronizationProvider.AcquireLeaseAsync(detailedJobName);

                Console.WriteLine($"Attempting to run job {detailedJobName} for DateTime {dateTime.RoundToPreviousInterval(schedule.Interval)}");
                await actionAsync(dateTime); //execute requested action
                Console.WriteLine($"Finished running run job {detailedJobName} for DateTime {dateTime.RoundToPreviousInterval(schedule.Interval)}");
                //notify the state provider that job has completed
                await schedule.SetLastExecutionAsync(detailedJobName, dateTime, leaseID);

                //notify sync provider that we do not longer need the lease/exclusive access
                await _synchronizationProvider.ReleaseLeaseAsync(detailedJobName, leaseID);
            }
        }
    }
}
