using Microsoft.EntityFrameworkCore;
using Nether.Data.Leaderboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Data.Sql.Leaderboard
{

    public class SaveScoreContext : DbContext
    {
        private readonly string _connectionString;
        private readonly string _table;

        public DbSet<SavedGamerScore> Scores { get; set; }

        public SaveScoreContext(string connectionString, string table)
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
    }

    public class SavedGamerScore
    {
        public int Score { get; set; }
        public string Gamertag { get; set; }
        public string CustomTag { get; set; }
    }
}

