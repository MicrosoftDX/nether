// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Itinero;
using Itinero.Exceptions;
using Itinero.IO.Osm;
using Itinero.LocalGeo;
using Itinero.Osm.Vehicles;
using Nether.Ingest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Nether.Test.ConsoleClient
{
    public class SimulateMovementMenu : ConsoleMenu
    {
        private IAnalyticsClient _client;

        private const float MinLatStockholm = 59.2983105f;
        private const float MinLonStockholm = 18.0204576f;
        private const float MaxLatStockholm = 59.3476538f;
        private const float MaxLonStockholm = 18.1090349f;

        private const string DefaultOsmPbfFilePath = @"c:\tmp\sweden-latest.osm.pbf";
        private const string DefaultRoutesFolder = @"c:\tmp\routes";
        private const string DefaultFilePrefix = "Sthlm";
        private const int DefaultNumberOfRoutes = 10000;

        private static RouterDb s_routerDb;
        private static Router s_router;
        private Random _rnd = new Random();

        public SimulateMovementMenu(IAnalyticsClient client)
        {
            _client = client;

            Title = "Nether Analytics Test Client - Simulate moving game client";

            MenuItems.Add('1', new ConsoleMenuItem("Generate random walking routes to files", GenerateRoutes));
            MenuItems.Add('2', new ConsoleMenuItem("Simulate walkers", SimulateWalkers));
        }

        private void SimulateWalkers()
        {
            var n = DateTime.UtcNow;
            var to = new DateTime(n.Year, n.Month, n.Day, 0, 0, 0, DateTimeKind.Utc);
            var from = to - TimeSpan.FromDays(1);

            var fromDate = ConsoleEx.ReadLine("From (UTC)", from);
            var toDate = ConsoleEx.ReadLine("To (UTC)", to);
            var timeZoneDiff = ConsoleEx.ReadLine("Time Zone Diff (h)", 1);
            var warmupTime = ConsoleEx.ReadLine("Warmup Time (min)", 15);
            var gamerTagFile1 = ConsoleEx.ReadLine("GamerTags File 1", "DataFiles/GamerTags1.txt", s => File.Exists(s));
            var gamerTagFile2 = ConsoleEx.ReadLine("GamerTags File 2", "DataFiles/GamerTags2.txt", s => File.Exists(s));
            var gamerTagFile3 = ConsoleEx.ReadLine("GamerTags File 3", "DataFiles/GamerTags3.txt", s => File.Exists(s));
            var playerDistributionFile = ConsoleEx.ReadLine("Player Distribution File", "DataFiles/PlayerDistribution.tsv", s => File.Exists(s));
            var routesFolder = ConsoleEx.ReadLine("GeoJson Routes Folder", DefaultRoutesFolder, s => Directory.Exists(s));
            var step = ConsoleEx.ReadLine("Increase Clock With (s)", 12);

            var gamerTagProvider = new GamerTagProvider(gamerTagFile1, gamerTagFile2, gamerTagFile3);
            var gameSessionProvider = new GameSessionProvider();
            var geoRouteProvider = new GeoRouteProvider(routesFolder);


            var playerDistribution = new PlayerDistributionProvider(playerDistributionFile);
            var now = fromDate.AddMinutes(-warmupTime);
            var bots = new List<NetherBot>();
            var botsNeeded = 0d;

            var lastTick = now.AddSeconds(-1);
            while (now < toDate)
            {
                bool warmup = now <= fromDate;

                if (now.Second == 0)
                {
                    Console.WriteLine();
                    Console.WriteLine(now.ToString());
                    Console.WriteLine($"Number of players: {bots.Count}");
                }

                Console.Write(".");
                var delta = now - lastTick;

                botsNeeded += playerDistribution.GetDistribution(now) / 60; // * delta.TotalMinutes;
                var botsToSpawnNow = (int)botsNeeded;

                // Add any new bots needed
                for (int i = 0; i < botsToSpawnNow; i++)
                {
                    var bot = new GeoBot(_client, gamerTagProvider, gameSessionProvider, geoRouteProvider);

                    bots.Add(bot);
                }

                // Deduct the number of bots that was just spawned from the needed count of bots
                botsNeeded -= botsToSpawnNow;

                // Update each bot
                foreach (var bot in bots)
                {
                    bot.TickAsync(now, warmup).Wait();
                }

                // Remove bots that are done
                bots.RemoveAll(b => b.SessionEnded);

                now += TimeSpan.FromSeconds(step);
            }

            _client.FlushAsync().Wait();
        }

        private void GenerateRoutes()
        {
            var outputPath = ConsoleEx.ReadLine("Output Directory", DefaultRoutesFolder, (s) => Directory.Exists(s));
            var outputFilePrefix = ConsoleEx.ReadLine("Output File Prefix", DefaultFilePrefix);

            var minLat = ConsoleEx.ReadLine("Min Latitude", MinLatStockholm);
            var maxLat = ConsoleEx.ReadLine("Max Latitude", MaxLatStockholm);
            var minLon = ConsoleEx.ReadLine("Min Longitude", MinLonStockholm);
            var maxLon = ConsoleEx.ReadLine("Max Longitude", MaxLonStockholm);

            var maxDistance = ConsoleEx.ReadLine("Max Distance (m), 0=unlimited", 0);
            var maxTotalWalkingTime = ConsoleEx.ReadLine("Max Walking Time (s), 0=unlimited", 0);

            var noRoutes = ConsoleEx.ReadLine("Number of routes to generate", DefaultNumberOfRoutes);

            var rnd = new Random();


            if (s_router == null)
            {
                var osmPbfPath = ConsoleEx.ReadLine("OpenStreetMap, OSM, PFB-file", DefaultOsmPbfFilePath, (s) => File.Exists(s));

                s_routerDb = new RouterDb();
                s_router = new Router(s_routerDb);

                using (var stream = File.OpenRead(osmPbfPath))
                {
                    Console.WriteLine("Loading Open Street Map Database. This can take a while!");
                    var loadOsmDataTask = Task.Run(() =>
                    {
                        s_routerDb.LoadOsmData(stream, Vehicle.Pedestrian);
                    });

                    while (!loadOsmDataTask.IsCompleted)
                    {
                        Console.Write(".");
                        Thread.Sleep(1000);
                    }
                }
                Console.WriteLine();
            }

            Console.WriteLine("Generating random routes");


            for (int i = 0; i < noRoutes; i++)
            {
                GenerateRandomRoute(i, minLat, maxLat, minLon, maxLon, maxDistance, maxTotalWalkingTime, outputPath, outputFilePrefix);

                if (Console.KeyAvailable)
                {
                    if (Console.ReadKey().Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                }
            }
        }


        private void GenerateRandomRoute(int i, float minLat, float maxLat, float minLon, float maxLon, int maxDistance, int maxTotalWalkingTime, string outputPath, string outputFilePrefix, bool verbose = true)
        {
            Console.WriteLine($"Route - {i}");

            Coordinate startCoordinate;
            Coordinate endCoordinate;
            Route route;

            Console.Write(".");
            while (true)
            {
                try
                {
                    startCoordinate = new Coordinate(
                        (float)(minLat + Math.Abs(maxLat - minLat) * _rnd.NextDouble()),
                        (float)(minLon + Math.Abs(maxLon - minLon) * _rnd.NextDouble()));

                    endCoordinate = new Coordinate(
                        (float)(minLat + Math.Abs(maxLat - minLat) * _rnd.NextDouble()),
                        (float)(minLon + Math.Abs(maxLon - minLon) * _rnd.NextDouble()));

                    route = s_router.Calculate(Vehicle.Pedestrian.Shortest(), startCoordinate, endCoordinate);

                    if (maxDistance > 0 && route.TotalDistance > maxDistance)
                    {
                        if (verbose) Console.Write("D");
                        continue;
                    }

                    if (maxTotalWalkingTime > 0 && route.TotalTime > maxTotalWalkingTime)
                    {
                        if (verbose) Console.Write("T");
                        continue;
                    }

                    break;
                }
                catch (ResolveFailedException)
                {
                    if (verbose) Console.Write("!");
                    continue;
                }
                catch (RouteNotFoundException)
                {
                    if (verbose) Console.Write("#");
                    continue;
                }
                catch (Exception)
                {
                    if (verbose) Console.Write("E");
                    continue;
                }
            }
            if (verbose) Console.WriteLine();

            var id = RandomEx.GetUniqueShortId();
            var filePath = Path.Combine(outputPath, $"{outputFilePrefix}_{id}.json");

            File.WriteAllText(filePath, route.ToGeoJson());

            if (verbose) Console.WriteLine($"Total Distance: {route.TotalDistance / 1000:0.0}km");
            if (verbose) Console.WriteLine($"Total Time:     {TimeSpan.FromSeconds(route.TotalTime)}");
            if (verbose) Console.WriteLine($"File:           {filePath}");
            if (verbose) Console.WriteLine();
        }
    }
}
