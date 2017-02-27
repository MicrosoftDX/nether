using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Nether.Data.Sql.Analytics.Migrations
{
    public partial class InitialAnalyticsContextMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyActiveSessions",
                columns: table => new
                {
                    EventDate = table.Column<DateTime>(nullable: false),
                    ActiveSessions = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyActiveSessions", x => x.EventDate);
                });

            migrationBuilder.CreateTable(
                name: "DailyActiveUsers",
                columns: table => new
                {
                    EventDate = table.Column<DateTime>(nullable: false),
                    ActiveUsers = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyActiveUsers", x => x.EventDate);
                });

            migrationBuilder.CreateTable(
                name: "DailyDurations",
                columns: table => new
                {
                    AverageGenericDuration = table.Column<long>(nullable: false),
                    EventDate = table.Column<DateTime>(nullable: false),
                    DisplayName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyDurations", x => new { x.AverageGenericDuration, x.EventDate });
                });

            migrationBuilder.CreateTable(
                name: "DailyGameDurations",
                columns: table => new
                {
                    EventDate = table.Column<DateTime>(nullable: false),
                    AverageGameDuration = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyGameDurations", x => x.EventDate);
                });

            migrationBuilder.CreateTable(
                name: "DailyLevelDropoff",
                columns: table => new
                {
                    EventDate = table.Column<DateTime>(nullable: false),
                    ReachedLevel = table.Column<int>(nullable: false),
                    TotalCount = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyLevelDropoff", x => x.EventDate);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyActiveSessions",
                columns: table => new
                {
                    EventDate = table.Column<DateTime>(nullable: false),
                    ActiveSessions = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyActiveSessions", x => x.EventDate);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyActiveUsers",
                columns: table => new
                {
                    EventDate = table.Column<DateTime>(nullable: false),
                    ActiveUsers = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyActiveUsers", x => x.EventDate);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyDurations",
                columns: table => new
                {
                    AverageGenericDuration = table.Column<long>(nullable: false),
                    EventDate = table.Column<DateTime>(nullable: false),
                    DisplayName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyDurations", x => new { x.AverageGenericDuration, x.EventDate });
                });

            migrationBuilder.CreateTable(
                name: "MonthlyGameDurations",
                columns: table => new
                {
                    EventDate = table.Column<DateTime>(nullable: false),
                    AverageGameDuration = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyGameDurations", x => x.EventDate);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyLevelDropoff",
                columns: table => new
                {
                    EventDate = table.Column<DateTime>(nullable: false),
                    ReachedLevel = table.Column<int>(nullable: false),
                    TotalCount = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyLevelDropoff", x => x.EventDate);
                });

            migrationBuilder.CreateTable(
                name: "YearlyActiveSessions",
                columns: table => new
                {
                    Year = table.Column<int>(nullable: false),
                    ActiveSessions = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YearlyActiveSessions", x => x.Year);
                });

            migrationBuilder.CreateTable(
                name: "YearlyActiveUsers",
                columns: table => new
                {
                    Year = table.Column<int>(nullable: false),
                    ActiveUsers = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YearlyActiveUsers", x => x.Year);
                });

            migrationBuilder.CreateTable(
                name: "YearlyDurations",
                columns: table => new
                {
                    AverageGenericDuration = table.Column<long>(nullable: false),
                    Year = table.Column<int>(nullable: false),
                    DisplayName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YearlyDurations", x => new { x.AverageGenericDuration, x.Year });
                });

            migrationBuilder.CreateTable(
                name: "YearlyGameDurations",
                columns: table => new
                {
                    Year = table.Column<int>(nullable: false),
                    AverageGameDuration = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YearlyGameDurations", x => x.Year);
                });

            migrationBuilder.CreateTable(
                name: "YearlyLevelDropoff",
                columns: table => new
                {
                    Year = table.Column<int>(nullable: false),
                    ReachedLevel = table.Column<int>(nullable: false),
                    TotalCount = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YearlyLevelDropoff", x => x.Year);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyActiveSessions");

            migrationBuilder.DropTable(
                name: "DailyActiveUsers");

            migrationBuilder.DropTable(
                name: "DailyDurations");

            migrationBuilder.DropTable(
                name: "DailyGameDurations");

            migrationBuilder.DropTable(
                name: "DailyLevelDropoff");

            migrationBuilder.DropTable(
                name: "MonthlyActiveSessions");

            migrationBuilder.DropTable(
                name: "MonthlyActiveUsers");

            migrationBuilder.DropTable(
                name: "MonthlyDurations");

            migrationBuilder.DropTable(
                name: "MonthlyGameDurations");

            migrationBuilder.DropTable(
                name: "MonthlyLevelDropoff");

            migrationBuilder.DropTable(
                name: "YearlyActiveSessions");

            migrationBuilder.DropTable(
                name: "YearlyActiveUsers");

            migrationBuilder.DropTable(
                name: "YearlyDurations");

            migrationBuilder.DropTable(
                name: "YearlyGameDurations");

            migrationBuilder.DropTable(
                name: "YearlyLevelDropoff");
        }
    }
}
