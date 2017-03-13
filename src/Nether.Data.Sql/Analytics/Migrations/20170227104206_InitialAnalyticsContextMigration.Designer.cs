using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Nether.Data.EntityFramework.Analytics;

namespace Nether.Data.Sql.Analytics.Migrations
{
    [DbContext(typeof(SqlAnalyticsContext))]
    [Migration("20170227104206_InitialAnalyticsContextMigration")]
    partial class InitialAnalyticsContextMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Nether.Data.EntityFramework.Analytics.DailyActiveSessionsEntity", b =>
                {
                    b.Property<DateTime>("EventDate");

                    b.Property<int>("ActiveSessions");

                    b.HasKey("EventDate");

                    b.ToTable("DailyActiveSessions");

                    b.HasAnnotation("SqlServer:TableName", "DailyActiveSessions");
                });

            modelBuilder.Entity("Nether.Data.EntityFramework.Analytics.DailyActiveUsersEntity", b =>
                {
                    b.Property<DateTime>("EventDate");

                    b.Property<int>("ActiveUsers");

                    b.HasKey("EventDate");

                    b.ToTable("DailyActiveUsers");

                    b.HasAnnotation("SqlServer:TableName", "DailyActiveUsers");
                });

            modelBuilder.Entity("Nether.Data.EntityFramework.Analytics.DailyDurationsEntity", b =>
                {
                    b.Property<long>("AverageGenericDuration");

                    b.Property<DateTime>("EventDate");

                    b.Property<string>("DisplayName")
                        .IsRequired();

                    b.HasKey("AverageGenericDuration", "EventDate");

                    b.ToTable("DailyDurations");

                    b.HasAnnotation("SqlServer:TableName", "DailyDurations");
                });

            modelBuilder.Entity("Nether.Data.EntityFramework.Analytics.DailyGameDurationsEntity", b =>
                {
                    b.Property<DateTime>("EventDate");

                    b.Property<long>("AverageGameDuration");

                    b.HasKey("EventDate");

                    b.ToTable("DailyGameDurations");

                    b.HasAnnotation("SqlServer:TableName", "DailyGameDurations");
                });

            modelBuilder.Entity("Nether.Data.EntityFramework.Analytics.DailyLevelDropOffEntity", b =>
                {
                    b.Property<DateTime>("EventDate");

                    b.Property<int>("ReachedLevel");

                    b.Property<long>("TotalCount");

                    b.HasKey("EventDate");

                    b.ToTable("DailyLevelDropOff");

                    b.HasAnnotation("SqlServer:TableName", "DailyLevelDropoff");
                });

            modelBuilder.Entity("Nether.Data.EntityFramework.Analytics.MonthlyActiveSessionsEntity", b =>
                {
                    b.Property<DateTime>("EventDate");

                    b.Property<int>("ActiveSessions");

                    b.HasKey("EventDate");

                    b.ToTable("MonthlyActiveSessions");

                    b.HasAnnotation("SqlServer:TableName", "MonthlyActiveSessions");
                });

            modelBuilder.Entity("Nether.Data.EntityFramework.Analytics.MonthlyActiveUsersEntity", b =>
                {
                    b.Property<DateTime>("EventDate");

                    b.Property<int>("ActiveUsers");

                    b.HasKey("EventDate");

                    b.ToTable("MonthlyActiveUsers");

                    b.HasAnnotation("SqlServer:TableName", "MonthlyActiveUsers");
                });

            modelBuilder.Entity("Nether.Data.EntityFramework.Analytics.MonthlyDurationsEntity", b =>
                {
                    b.Property<long>("AverageGenericDuration");

                    b.Property<DateTime>("EventDate");

                    b.Property<string>("DisplayName")
                        .IsRequired();

                    b.HasKey("AverageGenericDuration", "EventDate");

                    b.ToTable("MonthlyDurations");

                    b.HasAnnotation("SqlServer:TableName", "MonthlyDurations");
                });

            modelBuilder.Entity("Nether.Data.EntityFramework.Analytics.MonthlyGameDurationsEntity", b =>
                {
                    b.Property<DateTime>("EventDate");

                    b.Property<long>("AverageGameDuration");

                    b.HasKey("EventDate");

                    b.ToTable("MonthlyGameDurations");

                    b.HasAnnotation("SqlServer:TableName", "MonthlyGameDurations");
                });

            modelBuilder.Entity("Nether.Data.EntityFramework.Analytics.MonthlyLevelDropOffEntity", b =>
                {
                    b.Property<DateTime>("EventDate");

                    b.Property<int>("ReachedLevel");

                    b.Property<long>("TotalCount");

                    b.HasKey("EventDate");

                    b.ToTable("MonthlyLevelDropOff");

                    b.HasAnnotation("SqlServer:TableName", "MonthlyLevelDropoff");
                });

            modelBuilder.Entity("Nether.Data.EntityFramework.Analytics.YearlyActiveSessionsEntity", b =>
                {
                    b.Property<int>("Year");

                    b.Property<int>("ActiveSessions");

                    b.HasKey("Year");

                    b.ToTable("YearlyActiveSessions");

                    b.HasAnnotation("SqlServer:TableName", "YearlyActiveSessions");
                });

            modelBuilder.Entity("Nether.Data.EntityFramework.Analytics.YearlyActiveUsersEntity", b =>
                {
                    b.Property<int>("Year");

                    b.Property<int>("ActiveUsers");

                    b.HasKey("Year");

                    b.ToTable("YearlyActiveUsers");

                    b.HasAnnotation("SqlServer:TableName", "YearlyActiveUsers");
                });

            modelBuilder.Entity("Nether.Data.EntityFramework.Analytics.YearlyDurationsEntity", b =>
                {
                    b.Property<long>("AverageGenericDuration");

                    b.Property<int>("Year");

                    b.Property<string>("DisplayName")
                        .IsRequired();

                    b.HasKey("AverageGenericDuration", "Year");

                    b.ToTable("YearlyDurations");

                    b.HasAnnotation("SqlServer:TableName", "YearlyDurations");
                });

            modelBuilder.Entity("Nether.Data.EntityFramework.Analytics.YearlyGameDurationsEntity", b =>
                {
                    b.Property<int>("Year");

                    b.Property<long>("AverageGameDuration");

                    b.HasKey("Year");

                    b.ToTable("YearlyGameDurations");

                    b.HasAnnotation("SqlServer:TableName", "YearlyGameDurations");
                });

            modelBuilder.Entity("Nether.Data.EntityFramework.Analytics.YearlyLevelDropOffEntity", b =>
                {
                    b.Property<int>("Year");

                    b.Property<int>("ReachedLevel");

                    b.Property<long>("TotalCount");

                    b.HasKey("Year");

                    b.ToTable("YearlyLevelDropOff");

                    b.HasAnnotation("SqlServer:TableName", "YearlyLevelDropoff");
                });
        }
    }
}
