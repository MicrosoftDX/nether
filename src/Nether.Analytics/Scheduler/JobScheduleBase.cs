// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    public class EveryHourOnTheHourSchedule : JobScheduleBase
    {
        public override JobInterval Interval { get { return JobInterval.Hourly; } }
        public EveryHourOnTheHourSchedule(IStateProvider stateProvider) : base(stateProvider)
        { }
    }

    public class EveryHalfHourOnTheHalfHourSchedule : JobScheduleBase
    {
        public override JobInterval Interval { get { return JobInterval.HalfHour; } }
        public EveryHalfHourOnTheHalfHourSchedule(IStateProvider stateProvider) : base(stateProvider)
        { }
    }

    public class EveryQuarterOnTheQuarterSchedule : JobScheduleBase
    {
        public override JobInterval Interval { get { return JobInterval.Quarterly; } }
        public EveryQuarterOnTheQuarterSchedule(IStateProvider stateProvider) : base(stateProvider)
        { }
    }

    public abstract class JobScheduleBase
    {
        private IStateProvider _stateProvider;

        public abstract JobInterval Interval { get; }

        public JobScheduleBase(IStateProvider stateProvider)
        {
            _stateProvider = stateProvider;
        }

        public async Task SetLastExecutionAsync(string jobName, DateTime dt, string leaseID)
        {
            await _stateProvider.SetLastExecutionDateTimeAsync(jobName, dt, leaseID);
        }

        /// <summary>
        /// Gets pending executions for this job
        /// If the job was last ran on 15.00, interval 15', current time is 15.31, it will return 15.15 & 15.30
        /// /// If the job was last ran on 15.00, interval 60', current time is 17.59, it will return 16.00 & 17.00
        /// </summary>
        /// <param name="jobName">Name of the job</param>
        /// <returns>A list containing missed opportunities</returns>
        public async Task<IEnumerable<DateTime>> GetPendingExecutionsAsync(string jobName)
        {
            DateTime? dt = await _stateProvider.GetLastExecutionDatetimeAsync(jobName);
            if (dt.HasValue)
            {
                var list = DateTimeUtilities.GetIntervalsBetween(DateTime.UtcNow, dt.Value, Interval);
                return list;
            }
            else //no datetime in state
            //so never executed before, so just return the last UTC.Now related interval
            //e.g. current UTC time is 17:38, Interval is 15, so return 17:30
            {
                return new List<DateTime> { DateTime.UtcNow.RoundToPreviousInterval(Interval) };
            }
        }
    }
}
