// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AnalyticsTestClient.Utils;
using NGeoHash;
using System;
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
            var startTime = new DateTime(2017, 01, 01, 12, 00, 00);
            var endTime = new DateTime(2017, 01, 01, 13, 00, 00);

            var walkers = new[]
            {
                StockholmWalker.OldTown001,
                StockholmWalker.OldTown002,
                StockholmWalker.OldTown003,
                StockholmWalker.OldTown004,
                StockholmWalker.OldTown005,
                StockholmWalker.OldTown006,
            };

            var simulator = new WalkerSimulator(
                startTime, endTime, TimeSpan.FromSeconds(20), 
                @"C:\tmp\000000.csv", walkers);
            simulator.RunSimulation();
        }
    }

    public class WalkerSimulator
    {
        DateTime _startTime;
        DateTime _endTime;
        TimeSpan _tickInterval;

        Walker[] _walkers;
        string _outputFileName;

        public WalkerSimulator(DateTime startTime, DateTime endTime, TimeSpan tickInterval, string outputFileName, params Walker[] walkers)
        {
            _startTime = startTime;
            _endTime = endTime;
            _tickInterval = tickInterval;
            _walkers = walkers;
            _outputFileName = outputFileName;

        }

        public void RunSimulation()
        {
            Console.WriteLine("Starting Walking Simulation");
            Console.WriteLine();
            var time = TimeSpan.FromSeconds(0);
            var tick = 0;

            using (var file = File.OpenWrite(_outputFileName))
            {
                using (var writer = new StreamWriter(file))
                {
                    while (_startTime + time < _endTime)
                    {
                        WalkTheWalkers(time, writer);

                        tick++;
                        time += _tickInterval;
                    }
                }
            }
            Console.WriteLine("Done with Walking Simulation");
            Console.WriteLine();
        }

        private void WalkTheWalkers(TimeSpan time, StreamWriter writer)
        {
            foreach (var walker in _walkers)
            {
                var line = walker.Walk(_startTime, time);
                if (!string.IsNullOrWhiteSpace(line))
                {
                    Console.WriteLine($"{time} - {line}");
                    writer.WriteLine(line);
                }
            }
        }
    }
}
