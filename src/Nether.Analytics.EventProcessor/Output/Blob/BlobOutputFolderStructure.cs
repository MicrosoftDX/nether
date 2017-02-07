using System;

namespace Nether.Analytics.EventProcessor.Output.Blob
{
    public static class BlobOutputFolderStructure
    {
        public static string YearMonthDayHourMinute()
        {
            return DateTime.UtcNow.ToString("yyyy/MM/dd/hh/mm");
        }

        public static string YearMonthDayHour()
        {
            var now = DateTime.UtcNow;
            return $"{now.Year:D4}/{now.Month:D2}/{now.Day:D2}/{now.Hour:D2}";
        }

        public static string YearMonthDay()
        {
            return DateTime.UtcNow.ToString("yyyy/MM/dd");
        }

        public static string YearMonth()
        {
            return DateTime.UtcNow.ToString("yyyy/MM");
        }

        public static string Year()
        {
            return DateTime.UtcNow.ToString("yyyy");
        }
    }
}