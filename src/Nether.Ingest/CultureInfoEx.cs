// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;

namespace Nether.Ingest
{
    public static class CultureInfoEx
    {
        public static void SetupNetherCultureInfo()
        {
            // Create a unique culture for all threads
            // in order to make sure serialization to and parsing from strings behaves similar.
            // Nether uses a modified en-US culture with the RoundTrip Serialization of DateTimes

            // Example, running on machine set to sv-SE:
            //
            // var d = 3.14159265359D;
            // var now = DateTime.Now;
            // var utcNow = DateTime.UtcNow;
            //
            // Console.WriteLine(d);
            // Console.WriteLine(now);
            // Console.WriteLine(utcNow);
            // Console.WriteLine(now.ToString("O"));
            // Console.WriteLine(utcNow.ToString("O"));
            //
            // Outputs:
            //
            // 3,14159265359
            // 2017-05-10 13:16:15
            // 2017-05-10 11:16:15
            // 2017-05-10T13:16:15.2588141+02:00
            // 2017-05-10T11:16:15.2598169Z

            var ci = new CultureInfo("en-US");
            ci.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            ci.DateTimeFormat.LongTimePattern = "THH:mm:ss.fffffffK";

            CultureInfo.DefaultThreadCurrentCulture = ci;

            // Example after custom CultureInfo is set
            //
            // Console.WriteLine(d);
            // Console.WriteLine(now);
            // Console.WriteLine(utcNow);
            //
            // Outputs:
            //
            // 3.14159265359 <- Notice that period is used instead of comma
            // 2017-05-10 T13:16:15.2588141+02:00
            // 2017-05-10 T11:16:15.2598169Z
            //
            //TODO: Figure out why we have an extra space between date and time
        }
    }
}
