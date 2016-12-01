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
            .HasKey(c => c.Gamertag);
            builder.Entity<SavedGamerScore>().ToTable(_table);

            builder.Entity<QueriedGamerScore>()
            .HasKey(c => c.Gamertag);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer(_connectionString);
        }

        public async Task SaveSoreAsync(GameScore score)
        {
            await Scores.AddAsync(new SavedGamerScore { Score = score.Score, CustomTag = score.CustomTag, Gamertag = score.Gamertag });
            await SaveChangesAsync();
        }

        public async Task<List<GameScore>> GetHighScoresAsync(int n)
        {
            string baseSql = " score, gamertag, customtag, rank() over(order by score desc) as ranking " +
                "from scores s1 where " +
                "score = (select max(score) from scores s2 where s1.gamertag = s2.gamertag)";
            string sql = n > 0 ? String.Concat("Select top ", n, baseSql) : String.Concat("Select ", baseSql);

            return await Ranks.FromSql(sql).Select(s =>
                new GameScore
                {
                    Score = s.Score,
                    Gamertag = s.Gamertag,
                    CustomTag = s.CustomTag,
                    Rank = s.Ranking
                }).ToListAsync();
        }

        public async Task<List<GameScore>> GetScoresAroundMeAsync(string gamerTag, long rank, int radius)
        {
            string sql = "select top " + radius + " * from (select score, gamertag, customtag, rank() over(order by score desc) as ranking " +
                         " from scores s1 where " +
                         " score = (select max(score) from scores s2 where s1.gamertag = s2.gamertag) " +
                         " ) as S where S.ranking >= {0} and S.gamertag != {1} " +
                         " union all " +
                         " select top " + radius + " * from(select score, gamertag, customtag, rank() over(order by score desc) as ranking " +
                         " from scores s1 where " +
                         " score = (select max(score) from scores s2 where s1.gamertag = s2.gamertag) " +
                         " ) as S where S.ranking < {0} ";

            return await Ranks.FromSql(sql, rank, gamerTag).Select(s =>
                new GameScore
                {
                    Score = s.Score,
                    Gamertag = s.Gamertag,
                    CustomTag = s.CustomTag,
                    Rank = s.Ranking
                }).ToListAsync();
            
        }

        public async Task<List<GameScore>> GetGamerRankAsync(string gamertag)
        {
            string sql = "select * from " +
                         " (select score, gamertag, customtag, rank() over(order by score desc) as ranking " +
                         " from scores s1 where " +
                         " score = (select max(score) from scores s2 where s1.gamertag = s2.gamertag) " +
                         " ) as Ranks where Ranks.gamertag = {0}";

            return await Ranks.FromSql(sql, gamertag).Select(s =>
                new GameScore
                {
                    Score = s.Score,
                    Gamertag = s.Gamertag,
                    CustomTag = s.CustomTag,
                    Rank = s.Ranking
                }).ToListAsync();
            
        }
    }

    public class SavedGamerScore
    {
        public int Score { get; set; }
        public string Gamertag { get; set; }
        public string CustomTag { get; set; }
    }

    public class QueriedGamerScore
    {
        public int Score { get; set; }
        public string Gamertag { get; set; }
        public string CustomTag { get; set; }
        public long Ranking { get; set; }
    }
}

