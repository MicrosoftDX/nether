// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Nether.Data.Sql.Leaderboard.Migrations
{
    public partial class InitialLeaderboardContextMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ranks",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    Ranking = table.Column<long>(nullable: false),
                    Score = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ranks", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Scores",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateAchieved = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(maxLength: 50, nullable: false),
                    Score = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scores", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Scores_DateAchieved_UserId_Score",
                table: "Scores",
                columns: new[] { "DateAchieved", "UserId", "Score" });


            migrationBuilder.Sql(@"
CREATE PROCEDURE [dbo].[GetPlayerRank]
	@UserId NVARCHAR(50),
	@Rank INT OUTPUT
AS
SET @Rank = -1

SELECT
	@Rank = Ranking
FROM (
	SELECT
		UserId,
		MAX(Score) AS Score,
		RANK() OVER (ORDER BY MAX(Score) DESC) AS Ranking
	FROM Scores
	GROUP BY UserId
) AS T
WHERE UserId = @UserId

RETURN 0
");

            migrationBuilder.Sql(@"
CREATE PROCEDURE [dbo].[GetHighScores]
	@StartRank int = 0,
	@Count int
AS
SELECT
	Score,
	UserId,
	Ranking
FROM
	(SELECT
		Score,
		UserId,
		RANK() OVER(ORDER BY Score DESC) AS Ranking
		FROM (
			SELECT
				UserId,
				MAX(Score) AS Score,
				MAX(CustomTag) AS CustomTag
			FROM Scores
			GROUP BY UserId
		) AS T
	) AS T2
WHERE Ranking BETWEEN @StartRank AND (@StartRank + @Count)
ORDER BY Ranking, UserId
RETURN 0
");


            migrationBuilder.Sql(@"
CREATE PROCEDURE[dbo].[GetScoresAroundPlayer]

    @UserId NVARCHAR(50),
	@Radius INT
AS
DECLARE @PlayerRank int

EXEC GetPlayerRank @UserId, @PlayerRank OUTPUT

IF(@PlayerRank >= 0)
BEGIN
    SELECT

        UserId,
        Score,
        Ranking

    FROM(
        SELECT
            UserId,
            MAX(Score) AS Score,
            RANK() OVER(ORDER BY MAX(Score) DESC) AS Ranking

        FROM Scores

        GROUP BY UserId
    ) AS T

    WHERE Ranking BETWEEN(@PlayerRank - @Radius) AND(@PlayerRank + @Radius)

    ORDER BY Ranking, UserId
END
ELSE
BEGIN
    SELECT
        UserId = NULL,
        Score = NULL,
        Ranking = NULL
    WHERE 1 = 0
END
RETURN 0
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE[dbo].[GetScoresAroundPlayer]");
            migrationBuilder.Sql("DROP PROCEDURE[dbo].[GetHighScores]");
            migrationBuilder.Sql("DROP PROCEDURE[dbo].[GetPlayerRank]");

            migrationBuilder.DropTable(
                name: "Ranks");

            migrationBuilder.DropTable(
                name: "Scores");
        }
    }
}
