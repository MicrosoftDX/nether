// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nether.Data.EntityFramework.Analytics;
using System.Reflection;

namespace Nether.Data.Sql.Analytics
{
    public class SqlAnalyticsContext : AnalyticsContextBase
    {
        private readonly SqlAnalyticsContextOptions _options;

        public SqlAnalyticsContext(ILoggerFactory loggerFactory, SqlAnalyticsContextOptions options)
            : base(loggerFactory)
        {
            _options = options;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<DailyActiveSessionsEntity>().ForSqlServerToTable("DailyActiveSessions");
            builder.Entity<MonthlyActiveSessionsEntity>().ForSqlServerToTable("MonthlyActiveSessions");
            builder.Entity<YearlyActiveSessionsEntity>().ForSqlServerToTable("YearlyActiveSessions");

            builder.Entity<DailyActiveUsersEntity>().ForSqlServerToTable("DailyActiveUsers");
            builder.Entity<MonthlyActiveUsersEntity>().ForSqlServerToTable("MonthlyActiveUsers");
            builder.Entity<YearlyActiveUsersEntity>().ForSqlServerToTable("YearlyActiveUsers");

            builder.Entity<DailyDurationsEntity>().ForSqlServerToTable("DailyDurations");
            builder.Entity<MonthlyDurationsEntity>().ForSqlServerToTable("MonthlyDurations");
            builder.Entity<YearlyDurationsEntity>().ForSqlServerToTable("YearlyDurations");

            builder.Entity<DailyGameDurationsEntity>().ForSqlServerToTable("DailyGameDurations");
            builder.Entity<MonthlyGameDurationsEntity>().ForSqlServerToTable("MonthlyGameDurations");
            builder.Entity<YearlyGameDurationsEntity>().ForSqlServerToTable("YearlyGameDurations");

            builder.Entity<DailyLevelDropOffEntity>().ForSqlServerToTable("DailyLevelDropoff");
            builder.Entity<MonthlyLevelDropOffEntity>().ForSqlServerToTable("MonthlyLevelDropoff");
            builder.Entity<YearlyLevelDropOffEntity>().ForSqlServerToTable("YearlyLevelDropoff");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);

            builder.UseSqlServer(_options.ConnectionString, options =>
            {
                options.MigrationsAssembly(typeof(SqlAnalyticsContext).GetTypeInfo().Assembly.GetName().Name);
            });
        }
    }

    public class SqlAnalyticsContextOptions
    {
        public string ConnectionString { get; set; }
    }
}