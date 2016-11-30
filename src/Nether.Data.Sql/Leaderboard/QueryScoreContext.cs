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
    public class QueryScoreContext : DbContext
    {
        private readonly string _connectionString;
        private readonly string _table;

        public DbSet<QueryResultGamerScore> Scores { get; set; }

        public QueryScoreContext(string connectionString, string table)
        {
            _connectionString = connectionString;
            _table = table;
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<QueryResultGamerScore>()
            .HasKey(c => c.Gamertag);

            builder.Entity<QueryResultGamerScore>().ToTable(_table);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer(_connectionString);
        }        

        public List<GameScore> GetHighScoresAsync(int n)
        {            
            string baseSql = " score, gamertag, customtag, rank() over(order by score desc) as ranking " +
                "from scores s1 where " + 
                "score = (select max(score) from scores s2 where s1.gamertag = s2.gamertag)";
            string sql = n > 0 ? String.Concat("Select top ", n, baseSql) : String.Concat("Select ", baseSql);

            return Scores.FromSql(sql).Select(s => 
                new GameScore {
                    Score = s.Score,
                    Gamertag = s.Gamertag,
                    CustomTag = s.CustomTag,
                    Rank = s.Ranking
                }).ToList();                       
        }

        internal List<GameScore> GetScoresAroundMe(string gamerTag, long rank, int radius)
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

            var res = Scores.FromSql(sql, rank, gamerTag).Select(s =>
                new GameScore
                {
                    Score = s.Score,
                    Gamertag = s.Gamertag,
                    CustomTag = s.CustomTag,
                    Rank = s.Ranking
                }).ToList();

            return res;
        }

        public List<GameScore> GetGamerRankAsync(string gamertag)
        {
            string sql = "select * from " +
                         " (select score, gamertag, customtag, rank() over(order by score desc) as ranking " +
                         " from scores s1 where " +
                         " score = (select max(score) from scores s2 where s1.gamertag = s2.gamertag) " +
                         " ) as Ranks where Ranks.gamertag = {0}";

            var res = Scores.FromSql(sql, gamertag).Select(s =>
                new GameScore
                {
                    Score = s.Score,
                    Gamertag = s.Gamertag,
                    CustomTag = s.CustomTag,
                    Rank = s.Ranking
                }).ToList();
            return res;
        }
    }

    public class QueryResultGamerScore
    {
        public int Score { get; set; }
        public string Gamertag { get; set; }
        public string CustomTag { get; set; }
        public long Ranking { get; set; }
    }
}
