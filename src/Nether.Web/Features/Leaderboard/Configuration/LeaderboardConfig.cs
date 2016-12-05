// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Web.Features.Leaderboard.Configuration
{
    public class Configuration
    {
        public static Dictionary<LeaderboardType, LeaderboardConfig> LeaderboardConfiguration = new Dictionary<LeaderboardType, LeaderboardConfig>()
        {
            {LeaderboardType.Default,  new LeaderboardConfig { AroundMe = false, Radius = 0, Top = 0 } },
            {LeaderboardType.Top,  new LeaderboardConfig { AroundMe = false, Radius = 0, Top = 5 } },
            {LeaderboardType.AroundMe,  new LeaderboardConfig { AroundMe = true, Radius = 2, Top = 0 } }
        };
    }

    public class LeaderboardConfig
    {
        public bool AroundMe { get; set; }
        public int Radius { get; set; }
        public int Top { get; set; }
    }

    /// <summary>
    /// Type of the leaderboard
    /// </summary>
    public enum LeaderboardType
    {
        /// <summary>
        /// Default leaderboard
        /// </summary>
        Default,

        Top,

        AroundMe
    }
}
