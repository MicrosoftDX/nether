// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nether.Analytics.UnitTests
{
    public class JobNever : IJobStateProvider
    {
        private JobNever()
        {
        }

        public static JobNever Executed
        {
            get { return new JobNever(); }
        }

        public Task DeleteEntryAsync(string blobName)
        {
            return Task.CompletedTask;
        }

        public async Task<DateTime?> GetLastExecutionDatetimeAsync(string jobName)
        {
            return await Task.FromResult<DateTime?>(null);
        }

        public Task SetLastExecutionDateTimeAsync(string jobName, DateTime dt, string leaseId)
        {
            return Task.CompletedTask;
        }
    }

    public class JobLastExecuted : IJobStateProvider
    {
        public async Task<DateTime?> GetLastExecutionDatetimeAsync(string jobName)
        {
            return await Task.FromResult(_lastExecuted);
        }

        public Task SetLastExecutionDateTimeAsync(string jobName, DateTime dt, string leaseId)
        {
            return Task.CompletedTask;
        }

        public Task DeleteEntryAsync(string blobName)
        {
            return Task.CompletedTask;
        }

        private readonly DateTime _lastExecuted;

        public static JobLastExecuted On(int year, int month, int dayOfMonth, int hours, int minutes)
        {
            return new JobLastExecuted(year, month, dayOfMonth, hours, minutes);
        }

        private JobLastExecuted(int year, int month, int dayOfMonth, int hours, int minutes)
        {
            _lastExecuted = new DateTime(year, month, dayOfMonth, hours, minutes, 0);
        }
    }

    public class CurrentUTCTime : ITimeProvider
    {
        public DateTime GetUtcNow()
        {
            return _dt;
        }

        private readonly DateTime _dt;
        private CurrentUTCTime(DateTime dt)
        {
            _dt = dt.AddSeconds(-dt.Second); //remove the seconds, just in case
        }

        public static CurrentUTCTime Is(int year, int month, int day, int hours, int minutes)
        {
            return new CurrentUTCTime(new DateTime(year, month, day, hours, minutes, 0));
        }
    }
}
