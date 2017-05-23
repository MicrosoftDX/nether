// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether.Analytics;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AnalyticsTestClient
{
    public class Program
    {
        public static Dictionary<string, object> PropertyCache = new Dictionary<string, object>();

        public static void Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine(@" _   _      _   _               ");
            Console.WriteLine(@"| \ | | ___| |_| |__   ___ _ __ ");
            Console.WriteLine(@"|  \| |/ _ \ __| '_ \ / _ \ '__|");
            Console.WriteLine(@"| |\  |  __/ |_| | | |  __/ |   ");
            Console.WriteLine(@"|_| \_|\___|\__|_| |_|\___|_|   ");
            Console.WriteLine(@"- Analytics Test Client -");
            Console.WriteLine();

            CultureInfoEx.SetupNetherCultureInfo();
            Config.Check();
            SetupPropertyCache();

            new MainMenu().Show();

            Console.WriteLine("Closing connection");
            EventHubManager.CloseConnectionToEventHub().Wait();
        }

        private static void SetupPropertyCache()
        {
            PropertyCache = new Dictionary<string, object>
            {
                {"GameSession", RandomEx.GetUniqueShortId() },
                {"EventCorrelationId", RandomEx.GetUniqueShortId()},
                {"GamerTag", "gamer" },
                {"DisplayName", "display" },
                {"Value", 1 }
            };
        }


    }
}