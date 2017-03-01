// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AnalyticsTestClient.Simulations;
using AnalyticsTestClient.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace AnalyticsTestClient
{
    public class SimulateMovementMenu : ConsoleMenu
    {
        public SimulateMovementMenu()
        {
            Title = "Nether Analytics Test Client - Simulate moving game client";

            MenuItems.Add('1', new ConsoleMenuItem("Generate simulated output (bypasses pipeline)", GenerateSimulatedOutput));
        }

        public void GenerateSimulatedOutput()
        {
            var startTime = new DateTime(2017, 01, 01, 00, 00, 00);
            var endTime = new DateTime(2017, 01, 31, 23, 59, 59);
            var currentStartTime = startTime;
            var rnd = new Random(0);
            var fileName = @"C:\tmp\000000.csv";

            //TODO: FIX THIS TO BE INTEGRATED IN SIMULATOR
            File.WriteAllText(fileName, "type|version|enqueueTime|dequeueTime|clientUtcTime|gameSessionId|lat|lon|geoHash|geoHashPrecision|geoHashCenterlat|geoHashCenterlon|country|district|city|properties" + Environment.NewLine);

            while (currentStartTime < endTime)
            {
                List<Walker> walkers = new List<Walker>();

                var walkersToChoseFrom = new[]
                    {
                    StockholmWalker.OldTown001,
                    StockholmWalker.OldTown002,
                    StockholmWalker.OldTown003,
                    StockholmWalker.OldTown004,
                    StockholmWalker.OldTown005,
                    StockholmWalker.OldTown006,
                    StockholmWalker.OldTown007
                    };

                foreach (var walker in walkersToChoseFrom)
                {
                    if (rnd.Next(100) > 50)
                        walkers.Add(walker);
                }


                var simulator = new WalkerSimulator(
                    currentStartTime, currentStartTime + TimeSpan.FromHours(1), TimeSpan.FromSeconds(10),
                    fileName, walkers.ToArray());
                simulator.RunSimulation();

                currentStartTime += TimeSpan.FromHours(1);
            }
        }
    }
}
