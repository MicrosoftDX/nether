// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.Extensions.Logging;
using Nether.Data.Leaderboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Data.Sql.Leaderboard
{
    public abstract class LeaderboardContextBase : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;
        public DbSet<SavedGamerScore> Scores { get; set; }

        public LeaderboardContextBase(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<SavedGamerScore>()
                .HasKey(c => c.Id);

            builder.Entity<SavedGamerScore>()
                .Property(s => s.Id).HasValueGenerator<GuidValueGenerator>();

            builder.Entity<SavedGamerScore>()
                .HasIndex(s => new { s.DateAchieved, s.Gamertag, s.Score });

            builder.Entity<SavedGamerScore>().Property(s => s.DateAchieved).IsRequired();
            builder.Entity<SavedGamerScore>().Property(s => s.Gamertag).IsRequired();
            builder.Entity<SavedGamerScore>().Property(s => s.Gamertag).HasMaxLength(50);
            builder.Entity<SavedGamerScore>().Property(s => s.CustomTag).HasMaxLength(50);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);

            builder.UseLoggerFactory(_loggerFactory);
        }


        public abstract Task<List<GameScore>> GetHighScoresAsync(int n);

        public abstract Task<List<GameScore>> GetScoresAroundMeAsync(string gamertag, int radius);


        public virtual async Task SaveScoreAsync(GameScore score)
        {
            await Scores.AddAsync(new SavedGamerScore { Score = score.Score, CustomTag = score.CustomTag, Gamertag = score.Gamertag, DateAchieved = DateTime.UtcNow });
            await SaveChangesAsync();
        }

        public async Task DeleteScores(string gamerTag)
        {
            List<SavedGamerScore> scores = await Scores.Where(_ => _.Gamertag == gamerTag).ToListAsync();
            RemoveRange(scores);
            await SaveChangesAsync();
        }
    }
}

