// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Data.Leaderboard
{
    public interface ILeaderboardConfiguration
    {
        LeaderboardConfig GetLeaderboardConfig(string name);
    }

    public enum LeaderboardType
    {
        All,
        Top,
        AroundMe
    }

    public class LeaderboardConfig
    {
        public string Name { get; set; }
        public LeaderboardType Type { get; set; }
        public int Top { get; set; }
        public int Radius { get; set; }
    }
}
