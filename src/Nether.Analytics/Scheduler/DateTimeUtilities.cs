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
        /// <param name="from">string in format yyyyMMdd-HHmm</param>
        /// <returns>A new DateTime from the parsed string</returns>
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
        /// Result is rounded to upper limit (via <see cref="Math.Ceiling(decimal)"/>)
        /// </summary>
        /// <param name="ts">TimeSpan</param>
        /// <param name="ji">Interval enumeration</param>
        /// <returns></returns>
        public static int CalculateTimePeriods(TimeSpan ts, JobInterval ji)
        {
            switch (ji)
            {
                case JobInterval.Daily: //60 minutes * 24 hours = a full day
                    return (int)Math.Ceiling(ts.TotalMinutes / (60 * 24));
                case JobInterval.Hourly:
                    return (int)Math.Ceiling(ts.TotalMinutes / 60);
                case JobInterval.Quarterly:
                    return (int)Math.Ceiling(ts.TotalMinutes / 15);
                case JobInterval.HalfHour:
                    return (int)Math.Ceiling(ts.TotalMinutes / 30);
                default:
                    throw new ArgumentException("Wrong JobInterval");
            }
        }

        /// <summary>
        /// Calculates the time intervals between two dateTime structs, depending on the <paramref name="interval"/> variable.
        /// Reason for <paramref name="includeFirstDateTime"/> existence is whether we want to keep and return the first result in the list.
        /// a) We want to keep it if we haven't run the job at this first interval (e.g. job never executed)
        /// b) We do not want to keep it if the job was last executed at that time
        /// </summary>
        /// <param name="presentTime">Current time (present)</param>
        /// <param name="pastTime">First interval (past)</param>
        /// <param name="includeFirstDateTime">True if we want to keep the first DateTime, false if not</param>
        /// <param name="interval">Interval enumeration</param>
        /// <returns>an enumerable collection of all datetime intervals</returns>
        public static IEnumerable<DateTime> GetIntervalsBetween(DateTime presentTime,
            DateTime pastTime, bool includeFirstDateTime, JobInterval interval)
        {
            pastTime = pastTime.RoundToPreviousInterval(interval);

            //get total TimeSpan
            TimeSpan ts = presentTime - pastTime;

            int timeperiods = CalculateTimePeriods(ts, interval);
            int minutes = interval.ToMinutes();
            List<DateTime> list = new List<DateTime>();
            //do we want the first interval or not???
            int startIndex = includeFirstDateTime ? 1 : 0;
            for (int i = startIndex; i < timeperiods; i++)
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
