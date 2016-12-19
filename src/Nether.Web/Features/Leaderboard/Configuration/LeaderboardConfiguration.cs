using Nether.Data.Leaderboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Web.Features.Leaderboard.Configuration
{
    public class LeaderboardConfiguration : ILeaderboardConfiguration
    {        
        private Dictionary<string, LeaderboardConfig> _leaderboards;
        
        public LeaderboardConfiguration(Dictionary<string, LeaderboardConfig> leaderboards)
        {
            _leaderboards = leaderboards;
        }

        public LeaderboardConfig GetLeaderboardConfig(string name)
        {
            LeaderboardConfig config = null;
            _leaderboards.TryGetValue(name, out config);
            return config;
        }
    }      
}
