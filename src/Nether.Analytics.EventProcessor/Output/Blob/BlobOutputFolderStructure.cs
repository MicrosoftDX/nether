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
            return DateTime.UtcNow.ToString("yyyy/MM/dd/hh");
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