//// Copyright (c) Microsoft. All rights reserved.
//// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Nether.Analytics
//{
//    /// <summary>
//    /// Schedule for a job that runs once per day at a specified HH:mm
//    /// Optionally accepts a DateTime <see cref="_firstExecutionRequest"/> for the first execution which
//    /// will be ignored in the case that this job has run before in the past
//    /// </summary>
//    public class RunOncePerDaySchedule : IJobSchedule
//    {
//        public JobInterval Interval
//        {
//            get { return JobInterval.Daily; }
//        }
//        private IJobStateProvider _stateProvider;
//        //_timeProvider is used via Reflection in a test. In case of refactoring, make sure you change it there, too
//        private ITimeProvider _timeProvider;

//        private DateTime _firstExecutionRequest;
//        private readonly int _minutes;
//        private readonly int _hours;

//        /// <summary>
//        /// Constructor for the RunOncePerDaySchedule class. It will schedule a job executed
//        /// daily at a specified interval. Optionally, user can request a DateTime for the
//        /// first execution. This is, however, relevant if the specific job has never run.
//        /// Bear in mind that jobs with the same name that are scheduled at different times
//        /// are considered different.
//        /// </summary>
//        /// <param name="stateProvider">Underlying state provider</param>
//        /// <param name="hours">Hour for the job to run</param>
//        /// <param name="minutes">Minutes for the job to run</param>
//        /// <param name="firstExecutionRequest">When to execute the job for the first time. If the job has ran before, this is ignored. Only the .Date portion of this DateTime parameter is taken into account</param>
//        /// <param name="timeProvider">Optional time provider, useful for unit tests. Defaults to system UTC time</param>
//        public RunOncePerDaySchedule(IJobStateProvider stateProvider, int hours, int minutes,
//            DateTime? firstExecutionRequest = null, ITimeProvider timeProvider = null)
//        {
//            _stateProvider = stateProvider ?? throw new ArgumentException($"{nameof(stateProvider)} cannot be null");

//            _timeProvider = timeProvider ?? new SystemTimeProvider();

//            if (hours < 0 || hours > 23)
//                throw new ArgumentException("hours must be between 0 and 23");
//            if (minutes < 0 || minutes > 59)
//                throw new ArgumentException("minutes must be between 0 and 59");
//            _hours = hours; _minutes = minutes;

//            if (firstExecutionRequest == null)
//            {
//                //no date given by the user so first execution will happen either on this day or the next one
//                //if requested time given is bigger than current time, then first execution will happen on the same day
//                //e.g. requested time is 4.00AM whereas current time is 3:55AM This means we want to run it in 5'
//                if (hours > _timeProvider.GetUtcNow().TimeOfDay.Hours && minutes > _timeProvider.GetUtcNow().TimeOfDay.Minutes)
//                    _firstExecutionRequest = _timeProvider.GetUtcNow().Date.AddHours(hours).AddMinutes(minutes);
//                //else if requested time is smaller than current time, this means we want the first execution to happen on the day after
//                //e.g. requested time is 4.00AM whereas current time is 4.05AM. So we'll run it on the next day
//                else
//                    _firstExecutionRequest = _timeProvider.GetUtcNow().Date.AddDays(1).AddHours(hours).AddMinutes(minutes);
//            }
//            //user has explicitly asked for a specific date on which to execute first, so take the year, month and day portions of his request into consideration
//            //hours and minutes portion comes from the "normal" arguments
//            else if (firstExecutionRequest.HasValue)
//            {
//                _firstExecutionRequest = new DateTime(firstExecutionRequest.Value.Year, firstExecutionRequest.Value.Month, firstExecutionRequest.Value.Day, hours, minutes, 0);
//            }
//        }



//        /// <summary>
//        /// Gets pending executions for this job
//        /// If the job was last ran on 15.00, interval 15', current time is 15.31, it will return 15.15 and 15.30
//        /// /// If the job was last ran on 15.00, interval 60', current time is 17.59, it will return 16.00 and 17.00
//        /// </summary>
//        /// <param name="detailedJobName">Name of the job</param>
//        /// <returns>A list containing missed opportunities</returns>
//        public async Task<IEnumerable<DateTime>> GetPendingExecutionsAsync(string detailedJobName)
//        {
//            DateTime? dt = await _stateProvider.GetLastExecutionDatetimeAsync(detailedJobName);
//            //since there is a value in the blob storage, the execution time requested by the user is completely ignored. sorry!
//            //calculation is done between current UTC time and value stored in the state store
//            if (dt.HasValue)
//            {
//                VerifyCorrectDateStoredInStorage(dt.Value); //probably unnecessary, but let's play it safe!
//                var list = DateTimeUtilities.GetIntervalsBetween(_timeProvider.GetUtcNow(), dt.Value, true, JobInterval.Daily);
//                return list;
//            }
//            else //no datetime in state so job has never executed before
//            {
//                var list = DateTimeUtilities.GetIntervalsBetween(_timeProvider.GetUtcNow(), _firstExecutionRequest, false, JobInterval.Daily);
//                return list;
//            }
//        }

//        /// <summary>
//        /// Sets the last execution time on the state provider
//        /// </summary>
//        /// <param name="detailedJobName">The "small" job name</param>
//        /// <param name="dt">DateTime of the job</param>
//        /// <param name="leaseId">The leaseId from the state provider</param>
//        /// <returns></returns>
//        public async Task SetLastExecutionAsync(string detailedJobName, DateTime dt, string leaseId)
//        {
//            await _stateProvider.SetLastExecutionDateTimeAsync(detailedJobName, dt, leaseId);
//        }

//        /// <summary>
//        /// Verifies that time of last execution stored in the storage is same as the one we have. Remember that two jobs with the same name but different execution times are considered different jobs (=> different detailedJobName)
//        /// </summary>
//        /// <param name="storageResult"></param>
//        private void VerifyCorrectDateStoredInStorage(DateTime storageResult)
//        {
//            if (storageResult.Hour != _hours || storageResult.Minute != _minutes)
//                throw new Exception("Underlying storage is possibly corrupt. Different time on storage entity name (e.g. blob) and on this entity's stored value");
//        }
//    }
//}
