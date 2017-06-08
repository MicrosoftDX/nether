// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Nether.Analytics
{
    public enum JobInterval
    {
        Hourly,
        Quarterly,
        HalfHour
    }

    public static class JobIntervalMethods
    {
        /// <summary>
        /// Handy method to convert JobInterval enum value to minutes
        /// </summary>
        /// <param name="ji"></param>
        /// <returns></returns>
        public static int ToMinutes(this JobInterval ji)
        {
            switch (ji)
            {
                case JobInterval.Hourly:
                    return 60;
                case JobInterval.Quarterly:
                    return 15;
                case JobInterval.HalfHour:
                    return 30;
                default:
                    throw new ArgumentException("Wrong JobInterval");
            }
        }
    }
}
