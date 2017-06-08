// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Nether.Analytics
{
    public static class DateTimeUtilities
    {
        /// <summary>
        /// Parses a new DateTime from format yyyyMMdd-HHmm
        /// </summary>
        /// <param name="from">DateTime in string format yyyyMMdd-HHmm</param>
        /// <returns>A new DateTime</returns>
        public static DateTime FromYMDHMString(string from)
        {
            return DateTime.ParseExact(from, "yyyyMMdd-HHmm", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a DateTime to format yyyyMMdd-HHmm
        /// </summary>
        /// <param name="dt">Provided DateTime</param>
        /// <returns>A string in format yyyyMMdd-HHmm</returns>
        public static string ToYMDHMSString(DateTime dt)
        {
            return dt.ToString("yyyyMMdd-HHmm");
        }

        /// <summary>
        /// Calculates time periods for a set timespan and interval
        /// For instance, if timespan is 3 hours and interval is 30 minutes, it will return 3 * 60 / 30 = 6
        /// </summary>
        /// <param name="ts">TimeSpan</param>
        /// <param name="ji">Interval enumeration</param>
        /// <returns></returns>
        public static int CalculateTimePeriods(TimeSpan ts, JobInterval ji)
        {
            switch (ji)
            {
                case JobInterval.Hourly:
                    return (int)(ts.TotalMinutes / 60);
                case JobInterval.Quarterly:
                    return (int)(ts.TotalMinutes / 15);
                case JobInterval.HalfHour:
                    return (int)(ts.TotalMinutes / 30);
                default:
                    throw new ArgumentException("Wrong JobInterval");
            }
        }

        /// <summary>
        /// This is used to find the missed opportunities. If a job was last run for 18.30 for 
        /// 30' interval and current time is 20.01, this returns a list of 19.00, 19.30, 20.00
        /// </summary>
        /// <param name="currentTime"></param>
        /// <param name="pastTime"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static IEnumerable<DateTime> GetIntervalsBetween(DateTime currentTime,
            DateTime pastTime, JobInterval interval)
        {
            pastTime = pastTime.RoundToPreviousInterval(interval);

            //get total TimeSpan
            TimeSpan ts = currentTime - pastTime;

            int timeperiods = CalculateTimePeriods(ts, interval);
            int minutes = interval.ToMinutes();
            List<DateTime> list = new List<DateTime>();
            //starting by one since we *always* want the first execution
            //to be the first after the last one (we do not want to re-run the job, of course!)
            for (int i = 1; i <= timeperiods; i++)
            {
                list.Add(pastTime.AddMinutes(minutes * i));
            }
            return list;
        }

        public static DateTime RoundToPreviousInterval(this DateTime dt, JobInterval interval)
        {
            //interval: 15, current minutes: 12 => startingMinutes 00
            //interval: 15, current minutes: 24 => startingMinutes 15
            //interval: 15, current minutes: 43 => startingMinutes 30
            //interval: 15, current minutes: 59 => startingMinutes 45
            //interval: 30, current minutes: 29 => startingMinutes 00
            //interval: 30, current minutes: 59 => startingMinutes 30
            //interval: 60, current minutes: X => startingMinutes 00
            int intervalMinutes = interval.ToMinutes();
            int startingMinutes = (dt.Minute / intervalMinutes) * intervalMinutes;
            //return provided datetime with minutes set as starting minutes
            //and secods set to zero
            return dt.AddMinutes(-dt.Minute).AddMinutes(startingMinutes).AddSeconds(-dt.Second);
        }
    }
}
