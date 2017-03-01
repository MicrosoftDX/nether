// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AnalyticsTestClient.Simulations;
using System;
using System.IO;

namespace AnalyticsTestClient
{
    public class WalkerSimulator
    {
        private DateTime _startTime;
        private DateTime _endTime;
        private TimeSpan _tickInterval;

        private Walker[] _walkers;
        private string _outputFileName;

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

            using (var file = File.Open(_outputFileName, FileMode.Append))
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
