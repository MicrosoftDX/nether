using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Web.Features.Leaderboard.Configuration
{
    public class Configuration
    {
        public static Dictionary<String, LeaderboardConfig> LeaderboardConfiguration = new Dictionary<string, LeaderboardConfig>()
        {
            {"Default",  new LeaderboardConfig { AroundMe = false, Radius = 0, Top = 0 } },
            {"Top",  new LeaderboardConfig { AroundMe = false, Radius = 0, Top = 5 } },
            {"AroundMe",  new LeaderboardConfig { AroundMe = true, Radius = 2, Top = 0 } }
        };
    }

    public class LeaderboardConfig
    {
        public bool AroundMe { get; set; }
        public int Radius { get; set; }
        public int Top { get; set; }
    }
}
