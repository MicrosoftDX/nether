// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Nether.Analytics.EventProcessor.Output.Blob
{
    public static class BlobOutputFolderStructure
    {
        public static string YearMonthDayHour(string gameEventType)
        {
            var now = DateTime.UtcNow;
            return $"{gameEventType}/{now.Year:D4}/{now.Month:D2}/{now.Day:D2}/{now.Hour:D2}/";
        }

        public static string YearMonthDay(string gameEventType)
        {
            var now = DateTime.UtcNow;
            return $"{gameEventType}/{now.Year:D4}/{now.Month:D2}/{now.Day:D2}/";
        }

        public static string YearMonth(string gameEventType)
        {
            var now = DateTime.UtcNow;
            return $"{gameEventType}/{now.Year:D4}/{now.Month:D2}/";
        }

        public static string Year(string gameEventType)
        {
            var now = DateTime.UtcNow;
            return $"{gameEventType}/{now.Year:D4}/";
        }
    }
}