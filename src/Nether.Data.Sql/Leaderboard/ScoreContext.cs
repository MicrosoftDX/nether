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

        private static string s_defaultSql = "select Score, GamerTag, CustomTag, row_number() over(order by Score desc) as Ranking from (select GamerTag, max(Score) as Score, max(CustomTag) as CustomTag from scores group by GamerTag) as T ";
        private static string s_topSql = "select top {0} Score, GamerTag, CustomTag, row_number() over(order by Score desc) as Ranking from (select GamerTag, max(Score) as Score, max(CustomTag) as CustomTag from scores group by GamerTag) as T order by Score desc";
        private static string s_aroundMeSql = "select top {0} * from(select score, gamertag, customtag, rank() over(order by score desc) as ranking from scores s1 where score = (select max(score) from scores s2 where s1.gamertag = s2.gamertag)) as S where S.ranking >={1} and S.gamertag != '{2}' union all select top {0} * from(select score, gamertag, customtag, rank() over(order by score desc) as ranking from scores s1 where score = (select max(score) from scores s2 where s1.gamertag = s2.gamertag)) as S where S.ranking< {1}";
        private static string s_gamerRankSql = "select * from (select score, gamertag, customtag, rank() over(order by score desc) as ranking from scores s1 where score = (select max(score) from scores s2 where s1.gamertag = s2.gamertag)) as Ranks where Ranks.gamertag = {0}";

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
            .HasKey(c => c.GamerTag);
            builder.Entity<SavedGamerScore>().ToTable(_table);

            builder.Entity<QueriedGamerScore>()
            .HasKey(c => c.GamerTag);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer(_connectionString);
        }

        public async Task SaveSoreAsync(GameScore score)
        {
            await Scores.AddAsync(new SavedGamerScore { Score = score.Score, CustomTag = score.CustomTag, GamerTag = score.GamerTag });
            await SaveChangesAsync();
        }

        public async Task<List<GameScore>> GetHighScoresAsync(int n)
        {
            string sql = n == 0 ? s_defaultSql : String.Format(s_topSql, n);            
            return await Ranks.FromSql(sql).Select(s =>
                new GameScore
                {
                    Score = s.Score,
                    GamerTag = s.GamerTag,
                    CustomTag = s.CustomTag,
                    Rank = s.Ranking
                }).ToListAsync();                            
        }

        public async Task<List<GameScore>> GetScoresAroundMeAsync(string gamerTag, long rank, int radius)
        {
            string sql = String.Format(s_aroundMeSql, radius, radius, gamerTag);
            return await Ranks.FromSql(sql).Select(s =>
                new GameScore
                {
                    Score = s.Score,
                    GamerTag = s.GamerTag,
                    CustomTag = s.CustomTag,
                    Rank = s.Ranking
                }).ToListAsync();           
        }

        public async Task<List<GameScore>> GetGamerRankAsync(string gamertag)
        {                        
            return await Ranks.FromSql(s_gamerRankSql, gamertag).Select(s =>
                new GameScore
                {
                    Score = s.Score,
                    GamerTag = s.GamerTag,
                    CustomTag = s.CustomTag,
                    Rank = s.Ranking
                }).ToListAsync();           
        }
    }

    public class SavedGamerScore
    {
        public int Score { get; set; }
        public string GamerTag { get; set; }
        public string CustomTag { get; set; }
    }

    public class QueriedGamerScore
    {
        public int Score { get; set; }
        public string GamerTag { get; set; }
        public string CustomTag { get; set; }
        public long Ranking { get; set; }
    }
}

