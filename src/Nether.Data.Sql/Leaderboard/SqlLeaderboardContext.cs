// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nether.Data.EntityFramework.Leaderboard;
using Nether.Data.Leaderboard;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace Nether.Data.Sql.Leaderboard
{
    public class SqlLeaderboardContext : LeaderboardContextBase
    {
        private readonly SqlLeaderboardContextOptions _options;

        private static string s_topSql = "EXEC GetHighScores @StartRank = 0, @Count = {0}";
        private static string s_aroundMeSql = "EXEC GetScoresAroundPlayer @UserId = {0}, @Radius = {1}";

        public DbSet<QueriedGamerScore> Ranks { get; set; }

        public SqlLeaderboardContext(ILoggerFactory loggerFactory, SqlLeaderboardContextOptions options)
            : base(loggerFactory)
        {
            _options = options;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<QueriedGamerScore>()
                .HasKey(c => c.UserId);

            builder.Entity<SavedGamerScore>()
                .ForSqlServerToTable("Scores");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer(_options.ConnectionString, options =>
            {
                options.MigrationsAssembly(this.GetType().GetTypeInfo().Assembly.GetName().Name);
                options.EnableRetryOnFailure();
            });
        }

        public override async Task<List<GameScore>> GetHighScoresAsync(int n)
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
                    UserId = s.UserId,
                    Rank = s.Ranking
                }).ToListAsync();
        }

        public override async Task<List<GameScore>> GetScoresAroundUserAsync(string userId, int radius)
        {
            return await Ranks.FromSql(s_aroundMeSql, userId, radius)
                .Select(s =>
                new GameScore
                {
                    Score = s.Score,
                    UserId = s.UserId,
                    Rank = s.Ranking
                }).ToListAsync();
        }
    }
    public class SqlLeaderboardContextOptions
    {
        public string ConnectionString { get; set; }
    }
}

