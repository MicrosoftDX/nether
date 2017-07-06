// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Reflection;

//namespace Nether.Ingest.UnitTests
//{
//    public static class Utilities
//    {
//        public static void ModifyMockSystemTime(RunOncePerDaySchedule schedule, DateTime dt)
//        {
//            var prop = schedule.GetType().GetField("_timeProvider", BindingFlags.NonPublic | BindingFlags.Instance);
//            prop.SetValue(schedule, CurrentUTCTime.Is(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute));
//        }
//    }
//}
