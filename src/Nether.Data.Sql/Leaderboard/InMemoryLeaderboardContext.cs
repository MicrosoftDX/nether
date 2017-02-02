// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nether.Data.Leaderboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Data.Sql.Leaderboard
{
    public class InMemoryLeaderboardContext : LeaderboardContextBase
    {
        public InMemoryLeaderboardContext(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseInMemoryDatabase();
        }

        public override async Task<List<GameScore>> GetHighScoresAsync(int n)
        {
            if (n == 0)
            {
                // temporary - set n to large number. Will remove this once we implement paging
                n = 1000;
            }

            return (await GetRankedScoresAsync())
                        .Take(n)
                        .ToList();
        }


        public override async Task<List<GameScore>> GetScoresAroundMeAsync(string gamertag, int radius)
        {
            var scores = await GetRankedScoresAsync(); // Naive implementation
            var gamerIndex = scores.FindIndex(s => s.GamerTag == gamertag);

            int startIndex = Math.Max(0, gamerIndex - radius);
            int endIndex = Math.Min(scores.Count, gamerIndex + radius);

            return scores
                        .Skip(startIndex)
                        .Take(endIndex - startIndex + 1)
                        .ToList();
        }
        private async Task<List<GameScore>> GetRankedScoresAsync()
        {
            // Hit a limit in the query parsing in EF in-memory store
            // Since this is just for local dev/test let's keep it simple and pull the list back to massage!
            var scoresList = await Scores.ToListAsync();
            return scoresList
                    .GroupBy(s => s.GamerTag)
                    .Select((g, index) => new GameScore
                    {
                        GamerTag = g.Key,
                        CustomTag = "TODO",// We haven't factored custom tags into the scores yet ;-)
                        Score = g.Max(s => s.Score),
                        Rank = index
                    })
                    .OrderByDescending(s=>s.Score)
                    .ToList();
        }
    }
}

