// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Nether.Data.Sql.Analytics
{
    public abstract class AnalyticsContextBase : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;

        public DbSet<DailyActiveSessionsEntity> DailyActiveSessions { get; set; }
        public DbSet<MonthlyActiveSessionsEntity> MonthlyActiveSessions { get; set; }
        public DbSet<YearlyActiveSessionsEntity> YearlyActiveSessions { get; set; }

        public DbSet<DailyActiveUsersEntity> DailyActiveUsers { get; set; }
        public DbSet<MonthlyActiveUsersEntity> MonthlyActiveUsers { get; set; }
        public DbSet<YearlyActiveUsersEntity> YearlyActiveUsers { get; set; }

        public DbSet<DailyDurationsEntity> DailyDurations { get; set; }
        public DbSet<MonthlyDurationsEntity> MonthlyDurations { get; set; }
        public DbSet<YearlyDurationsEntity> YearlyDurations { get; set; }

        public DbSet<DailyGameDurationsEntity> DailyGameDurations { get; set; }
        public DbSet<MonthlyGameDurationsEntity> MonthlyGameDurations { get; set; }
        public DbSet<YearlyGameDurationsEntity> YearlyGameDurations { get; set; }

        public DbSet<DailyLevelDropOffEntity> DailyLevelDropOff { get; set; }
        public DbSet<MonthlyLevelDropOffEntity> MonthlyLevelDropOff { get; set; }
        public DbSet<YearlyLevelDropOffEntity> YearlyLevelDropOff { get; set; }

        public AnalyticsContextBase(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<DailyActiveSessionsEntity>(o =>
            {
                o.HasKey(r => r.EventDate);
                o.Property(r => r.ActiveSessions).IsRequired();
            });
            builder.Entity<MonthlyActiveSessionsEntity>(o =>
            {
                o.HasKey(r => r.EventDate);
                o.Property(r => r.ActiveSessions).IsRequired();
            });
            builder.Entity<YearlyActiveSessionsEntity>(o =>
            {
                o.HasKey(r => r.Year);
                o.Property(r => r.Year).ValueGeneratedNever();
                o.Property(r => r.ActiveSessions).IsRequired();
            });


            builder.Entity<DailyActiveUsersEntity>(o =>
            {
                o.HasKey(r => r.EventDate);
                o.Property(r => r.ActiveUsers).IsRequired();
            });
            builder.Entity<MonthlyActiveUsersEntity>(o =>
            {
                o.HasKey(r => r.EventDate);
                o.Property(r => r.ActiveUsers).IsRequired();
            });
            builder.Entity<YearlyActiveUsersEntity>(o =>
            {
                o.HasKey(r => r.Year);
                o.Property(r => r.Year).ValueGeneratedNever();
                o.Property(r => r.ActiveUsers).IsRequired();
            });


            builder.Entity<DailyDurationsEntity>(o =>
            {
                o.HasKey(r => new { r.AverageGenericDuration, r.EventDate });
                o.Property(r => r.DisplayName).IsRequired();
                o.Property(r => r.AverageGenericDuration).IsRequired();
            });
            builder.Entity<MonthlyDurationsEntity>(o =>
            {
                o.HasKey(r => new { r.AverageGenericDuration, r.EventDate });
                o.Property(r => r.DisplayName).IsRequired();
                o.Property(r => r.AverageGenericDuration).IsRequired();
            });
            builder.Entity<YearlyDurationsEntity>(o =>
            {
                o.HasKey(r => new { r.AverageGenericDuration, r.Year });
                o.Property(r => r.Year).ValueGeneratedNever();
                o.Property(r => r.DisplayName).IsRequired();
                o.Property(r => r.AverageGenericDuration).IsRequired();
            });


            builder.Entity<DailyGameDurationsEntity>(o =>
            {
                o.HasKey(r => r.EventDate);
                o.Property(r => r.AverageGameDuration).IsRequired();
            });
            builder.Entity<MonthlyGameDurationsEntity>(o =>
            {
                o.HasKey(r => r.EventDate);
                o.Property(r => r.AverageGameDuration).IsRequired();
            });
            builder.Entity<YearlyGameDurationsEntity>(o =>
            {
                o.HasKey(r => r.Year);
                o.Property(r => r.Year).ValueGeneratedNever();
                o.Property(r => r.AverageGameDuration).IsRequired();
            });


            builder.Entity<DailyLevelDropOffEntity>(o =>
            {
                o.HasKey(r => r.EventDate);
                o.Property(r => r.ReachedLevel).IsRequired();
                o.Property(r => r.TotalCount).IsRequired();
            });
            builder.Entity<MonthlyLevelDropOffEntity>(o =>
            {
                o.HasKey(r => r.EventDate);
                o.Property(r => r.ReachedLevel).IsRequired();
                o.Property(r => r.TotalCount).IsRequired();
            });
            builder.Entity<YearlyLevelDropOffEntity>(o =>
            {
                o.HasKey(r => r.Year);
                o.Property(r => r.Year).ValueGeneratedNever();
                o.Property(r => r.ReachedLevel).IsRequired();
                o.Property(r => r.TotalCount).IsRequired();
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);

            builder.UseLoggerFactory(_loggerFactory);
        }
    }
}
