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
            var gamerIndex = scores.FindIndex(s => s.Gamertag == gamertag);

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

            // Not aiming for the most efficient in-memory operation here... ;-)

            // filter down to just the best score for each player
            var bestScoreList = scoresList
                    .GroupBy(s => s.GamerTag)
                    .Select(g => new
                    {
                        Gamertag = g.Key,
                        CustomTag = "TODO",// We haven't factored custom tags into the scores yet
                        Score = g.Max(s => s.Score),
                    });

            // now sort the list and track the ROW_NUMBER
            var sortedBestScoreListWithIndex = bestScoreList
                    .OrderByDescending(s => s.Score)
                    .Select((s, index)=> new
                    {
                        s.Gamertag,
                        s.CustomTag,
                        s.Score,
                        RowNumber = index + 1
                    })
                    .ToList();

            // group the scores to aid adding rank
            var groupedScores = sortedBestScoreListWithIndex.GroupBy(s => s.Score)
                .Select(g => new
                {
                    Score = g.Key,
                    StartRank = g.First().RowNumber,
                    Players = g.OrderBy(s => s.Gamertag)
                });

            // now unroll the groups applying the rank
            return groupedScores.SelectMany(g => g.Players.Select(p => new GameScore
            {
                Rank = g.StartRank,
                Gamertag = p.Gamertag,
                Score = p.Score,
                CustomTag = p.CustomTag
            })).ToList();
        }

    }
}

