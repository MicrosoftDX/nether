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

        public DbSet<GamerScore> Scores { get; set; }

        public ScoreContext(string connectionString, string table)
        {
            _connectionString = connectionString;
            _table = table;
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<GamerScore>()
            .HasKey(c => c.Gamertag);

            builder.Entity<GamerScore>().ToTable(_table);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer(_connectionString);
        }

        public async Task SaveSoreAsync(GameScore score)
        {
            Scores.Add(new GamerScore { Score = score.Score, CustomTag = score.CustomTag, Gamertag = score.Gamertag });
            await SaveChangesAsync();
        }

        public List<GameScore> GetHighScoresAsync(int n)
        {
            // currently returns default list of all players with their high score for the last 24H
            DateTime now = DateTime.UtcNow; // the date in the table is utc
            DateTime lastDay = now.AddHours(-24);

            string sql = "select {0} score, gamertag, customtag, rank() over (order by Score desc) as ranking from scores s1 where score = (select max(score) from  scores s2 where s1.gamertag = s2.gamertag and s1.DateAchieved between {1} and {2})";
            string top = n > 0 ? "top " + n : "";

            try
            {
                var res = Scores.FromSql(sql, top, lastDay.ToString(), now.ToString())
                    .ToList();

                return res.Select(s => new GameScore { Score = s.Score, Gamertag = s.Gamertag, CustomTag = s.CustomTag, Rank = s.Ranking }).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }

    public class GamerScore
    {
        public int Score { get; set; }
        public string Gamertag { get; set; }
        public string CustomTag { get; set; }
        public int Ranking { get; }
    }
}
