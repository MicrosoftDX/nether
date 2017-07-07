// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether.EventHubs;
using Nether.Ingest;
using System;
using System.Collections.Generic;

namespace Nether.Test.ConsoleClient
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
            Console.WriteLine(@"- Nether.Test.ConsoleClient -");
            Console.WriteLine();

            CultureInfoEx.SetupNetherCultureInfo();
            Config.Check();
            SetupPropertyCache();

            var client = new EventHubAnalyticsClient(Config.Root[Config.NAH_EHLISTENER_CONNECTIONSTRING], Config.Root[Config.NAH_EHLISTENER_EVENTHUBPATH]);

            new MainMenu(client).Show();

            Console.WriteLine("Closing connection");
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