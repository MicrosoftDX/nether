// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Nether.Data.Leaderboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Data.Sql.Leaderboard
{
    public class ScoreContext : DbContext
    {
        private readonly string _connectionString;
        private readonly string _table;

        private static string s_topSql = "EXEC GetHighScores @StartRank = 0, @Count = {0}";
        private static string s_aroundMeSql = "EXEC GetScoresAroundPlayer @Gamertag = {0}, @Radius = {1}";

        public DbSet<SavedGamerScore> Scores { get; set; }
        public DbSet<QueriedGamerScore> Ranks { get; set; }

        public ScoreContext(string connectionString, string table)
        {
            _connectionString = connectionString;
            _table = table;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<SavedGamerScore>()
                .HasKey(c => c.Id);

            builder.Entity<QueriedGamerScore>()
                .HasKey(c => c.GamerTag);


            builder.Entity<SavedGamerScore>()
                .ForSqlServerToTable(_table);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer(_connectionString);
        }

        public async Task SaveScoreAsync(GameScore score)
        {
            await Scores.AddAsync(new SavedGamerScore { Score = score.Score, CustomTag = score.CustomTag, GamerTag = score.GamerTag, DateAchieved = DateTime.UtcNow });
            await SaveChangesAsync();
        }

        public async Task<List<GameScore>> GetHighScoresAsync(int n)
        {
            if (n == 0)
            {
                // temporary - set n to large number. Will remove this once we implement paging
                n = 1000;
            }
            return await Ranks.FromSql(s_topSql, n)
                .Select(s =>
                new GameScore
                {
                    Score = s.Score,
                    GamerTag = s.GamerTag,
                    CustomTag = s.CustomTag,
                    Rank = s.Ranking
                }).ToListAsync();
        }

        public async Task<List<GameScore>> GetScoresAroundMeAsync(string gamertag, int radius)
        {
            return await Ranks.FromSql(s_aroundMeSql, gamertag, radius)
                .Select(s =>
                new GameScore
                {
                    Score = s.Score,
                    GamerTag = s.GamerTag,
                    CustomTag = s.CustomTag,
                    Rank = s.Ranking
                }).ToListAsync();
        }

        public async Task DeleteScores(string gamerTag)
        {
            List<SavedGamerScore> scores = await Scores.Where(_ => _.GamerTag == gamerTag).ToListAsync();
            RemoveRange(scores);
            await SaveChangesAsync();
        }
    }

    public class SavedGamerScore
    {
        public Guid Id { get; set; }
        public int Score { get; set; }
        public string GamerTag { get; set; }
        public string CustomTag { get; set; }
        public DateTime DateAchieved { get; set; }
    }

    public class QueriedGamerScore
    {
        public int Score { get; set; }
        public string GamerTag { get; set; }
        public string CustomTag { get; set; }
        public long Ranking { get; set; }
    }
}

