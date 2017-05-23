// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace AnalyticsTestClient
{
    public static class RandomEx
    {
        private static Random s_random = new Random();

        public static S TakeRandom<S>(this S[] array)
        {
            if (array == null || array.Length == 0)
                return default(S);

            return array[s_random.Next(array.Length)];
        }

        public static string GetUniqueShortId()
        {
            var i = 1L;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }

        public static TimeSpan WithRandomDeviation(this TimeSpan t, int maxDeviationInSeconds = 5)
        {
            return t + System.TimeSpan.FromSeconds(s_random.Next(-maxDeviationInSeconds, maxDeviationInSeconds));
        }

        public static TimeSpan WithRandomDeviation(this TimeSpan t, TimeSpan maxDeviation)
        {
            return WithRandomDeviation(t, (int)maxDeviation.TotalSeconds);
        }

        public static TimeSpan TimeSpan(TimeSpan from, TimeSpan to)
        {
            return System.TimeSpan.FromSeconds((to - from).TotalSeconds * s_random.NextDouble());
        }
    }
}
