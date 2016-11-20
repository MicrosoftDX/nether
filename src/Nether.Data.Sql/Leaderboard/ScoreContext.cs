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
            await Scores.AddAsync(new GamerScore { Score = score.Score, CustomTag = score.CustomTag, Gamertag = score.Gamertag });
            await SaveChangesAsync();
        }

        public async Task<List<GameScore>> GetHighScoresAsync()
        {
            // currently returns default list of all players with their high score for the last 24H
            DateTime now = DateTime.UtcNow; // the date in the table is utc
            DateTime lastDay = now.AddHours(-24);
                       
            // TODO: consider swithiching to linq in DateAchieved will be part of the GamerScore record
            var res = Scores.FromSql("select max(score) as score, gamertag , customtag from Scores where DateAchieved between {0} and {1} group by gamertag, customtag", lastDay.ToString(), now.ToString())
                .ToList().Select(s => new GameScore { Gamertag = s.Gamertag, Score = s.Score })
                .ToList();
            return res;
        }
    }

    public class GamerScore
    {
        public int Score { get; set; }
        public string Gamertag { get; set; }
        public string CustomTag { get; set; }

    }
}
