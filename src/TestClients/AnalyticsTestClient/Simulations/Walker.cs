// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NGeoHash;
using System;
using System.Linq;
using System.Collections.Generic;

namespace AnalyticsTestClient.Simulations
{
    public class Walker
    {
        string _gameSessionId;
        Random _random;
        double _activeness;

        GeoCheckpoint[] _checkpoints;

        public Walker(string gameSessionId, int randomSeed, double activeness, params GeoCheckpoint[] checkpoints)
        {
            _gameSessionId = gameSessionId;
            _random = new Random(randomSeed);
            _activeness = activeness;

            // Go throught and check that all checkpoints have times set
            // Otherwise add 30 seconds from lastCheckpoint on all checkpoints
            List<GeoCheckpoint> tmpCheckpoints = new List<GeoCheckpoint>();
            GeoCheckpoint lastCheckpoint = null;

            foreach (var checkpoint in checkpoints)
            {
                if (lastCheckpoint == null)
                {
                    tmpCheckpoints.Add(checkpoint);
                    lastCheckpoint = checkpoint;
                }
                else
                {
                    if (checkpoint.AtTime.TotalSeconds == 0)
                    {
                        var adjustedCheckpoint = new GeoCheckpoint(checkpoint.Position, lastCheckpoint.AtTime + TimeSpan.FromSeconds(30));
                        tmpCheckpoints.Add(adjustedCheckpoint);
                        lastCheckpoint = adjustedCheckpoint;
                    }
                    else
                    {
                        tmpCheckpoints.Add(checkpoint);
                        lastCheckpoint = checkpoint;
                    }
                }
            }

            _checkpoints = tmpCheckpoints.ToArray();
        }

        public string GameSessionId => _gameSessionId;
        public GeoCheckpoint[] Checkpoints => _checkpoints;

        public string Walk(DateTime startTime, TimeSpan time)
        {
            // Should be active
            if (_random.NextDouble() < _activeness)
            {
                var pos = CalculatePosition(time);
                if (pos == null)
                    return "";

                var line = Print(startTime, time, pos);
                return line;
            }
            else
            {
                return "";
            }
        }

        private string Print(DateTime startTime, TimeSpan time, GeoPosition pos)
        {
            const string type = "location";
            const string version = "1.0.0";

            var enqueueTime = startTime + time;
            var dequeueTime = enqueueTime.AddMilliseconds(_random.Next(500));
            var clientUtcTime = enqueueTime.AddMilliseconds(-_random.Next(500));
            // "yyyy-MM-dd HH:mm:ss"
            var hashPos = GeoHash.DecodeInt(pos.GeoHash, 32);

            var line = $"{type}|{version}|{enqueueTime:yyyy-MM-dd HH:mm:ss}|{dequeueTime:yyyy-MM-dd HH:mm:ss}|{clientUtcTime:yyyy-MM-dd HH:mm:ss}|{_gameSessionId}|{pos.Lat:0.############}|{pos.Lon:0.############}|{pos.GeoHash}|{pos.Precision}|{hashPos.Coordinates.Lat}|{hashPos.Coordinates.Lon}|Country N/A|District N/A|City N/A|prop1=tst;prop2=tmp";

            return line;
        }

        private GeoPosition CalculatePosition(TimeSpan time)
        {
            var prevCheckpoint = (from gcp in _checkpoints
                                  where gcp.AtTime <= time
                                  orderby gcp.AtTime
                                  select gcp).LastOrDefault();

            var nextCheckpoint = (from gcp in _checkpoints
                                  where gcp.AtTime > time
                                  orderby gcp.AtTime
                                  select gcp).FirstOrDefault();

            if (prevCheckpoint == null)
            {
                // Walker hasn't started walking yet
                return null;
            }

            if (nextCheckpoint == null)
            {
                // Walker has stoped walking
                return null;
            }

            var periodLength = nextCheckpoint.AtTime - prevCheckpoint.AtTime;
            var timeInPeriod = time - prevCheckpoint.AtTime;
            var percentageInPeriod = timeInPeriod.TotalMilliseconds / periodLength.TotalMilliseconds;

            var lat = prevCheckpoint.Position.Lat + (nextCheckpoint.Position.Lat - prevCheckpoint.Position.Lat) * percentageInPeriod;
            var lon = prevCheckpoint.Position.Lon + (nextCheckpoint.Position.Lon - prevCheckpoint.Position.Lon) * percentageInPeriod;

            return new GeoPosition(lat, lon);
        }
    }
}
