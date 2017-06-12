// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    public class JobExecutor
    {
        private readonly ISynchronizationProvider _synchronizationProvider;
        private IJobStateProvider _jobStateProvider;

        public JobExecutor(ISynchronizationProvider synchronizationProvider, IJobStateProvider jobStateProvider)
        {
            _synchronizationProvider = synchronizationProvider;
            _jobStateProvider = jobStateProvider;
        }

        public async Task<bool> RunAsSingletonAsync(string jobId, IJobScheduleV2 schedule, Func<DateTime, Task> actionAsync, DateTime? startTime = null)
        {
            //TODO: Update to new nicer syntax as soon as we get CodeFormatter to support C#7 syntax
            var t = await _synchronizationProvider.TryAcquireLeaseAsync(jobId);
            var acquiredLease = t.Item1;
            var leaseId = t.Item2;

            if (!acquiredLease)
                return false;

            try
            {
                using (var timer = new Timer(RenewLease, Tuple.Create(jobId, leaseId), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5)))
                {
                    var lastExecutionTime = await _jobStateProvider.GetLastExecutionDatetimeAsync(jobId) ?? (startTime ?? DateTime.UtcNow);
                    var executionTime = lastExecutionTime;

                    while ((executionTime = schedule.GetNextExcecutionTime(executionTime)) <= DateTime.UtcNow)
                    {
                        await actionAsync(executionTime);
                        await _jobStateProvider.SetLastExecutionDateTimeAsync(jobId, executionTime, leaseId);
                    }
                }
            }
            finally
            {
                await _synchronizationProvider.ReleaseLeaseAsync(jobId, leaseId);
            }

            return true;
        }

        private void RenewLease(object state)
        {
            var t = state as Tuple<string, string>;
            var jobId = t.Item1;
            var leaseId = t.Item2;

            Console.WriteLine("Renewing lease");
            _synchronizationProvider.RenewLeaseAsync(jobId, leaseId).Wait();
        }
    }
}
