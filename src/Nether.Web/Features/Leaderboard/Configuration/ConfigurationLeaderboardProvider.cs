// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Nether.Data.Leaderboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Web.Features.Leaderboard.Configuration
{
    public class ConfigurationLeaderboardProvider : ILeaderboardProvider
    {
        private Dictionary<string, LeaderboardConfig> _leaderboards;

        public ConfigurationLeaderboardProvider(IConfiguration configuration)
        {
            _leaderboards = LoadLeaderboards(configuration.GetSection("Leaderboard:Leaderboards"));
        }

        public IEnumerable<LeaderboardConfig> GetAll()
        {
            return _leaderboards.Values.OrderBy(l => l.Name);
        }

        public LeaderboardConfig GetByName(string name)
        {
            LeaderboardConfig config = null;
            _leaderboards.TryGetValue(name, out config);
            return config;
        }


        private static Dictionary<string, LeaderboardConfig> LoadLeaderboards(IConfigurationSection configuration)
        {
            Dictionary<string, LeaderboardConfig> leaderboards = new Dictionary<string, LeaderboardConfig>();
            // go over all leaderboards under "Leaderboard:Leaderboards"
            foreach (var config in configuration.GetChildren())
            {
                string name = config.Key;
                bool includeCurrentPlayer = bool.Parse(config["IncludeCurrentPlayer"] ?? "false");
                LeaderboardType type = (LeaderboardType)Enum.Parse(typeof(LeaderboardType), config["Type"]);
                LeaderboardConfig leaderboardConfig = new LeaderboardConfig
                {
                    Name = name,
                    Type = type,
                    IncludeCurrentPlayer = includeCurrentPlayer
                };

                switch (type)
                {
                    case LeaderboardType.Top:
                        string top = config["Top"];
                        if (top == null)
                        {
                            throw new Exception($"Leaderboard type Top must have Top value set. Leaderboard name: '{name}'");
                        }
                        else
                        {
                            leaderboardConfig.Top = int.Parse(top);
                        }
                        break;
                    case LeaderboardType.AroundMe:
                        string radius = config["Radius"];
                        if (radius == null)
                        {
                            throw new Exception($"Leaderboard type AroundMe must have Radius value set. Leaderboard name: '{name}'");
                        }
                        else
                        {
                            leaderboardConfig.Radius = int.Parse(radius);
                        }
                        break;
                    case LeaderboardType.All:
                        break;
                }

                leaderboards.Add(name, leaderboardConfig);
            }

            return leaderboards;
        }
    }
}
