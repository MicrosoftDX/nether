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
                    Gamertag = table.Column<string>(nullable: false),
                    CustomTag = table.Column<string>(nullable: true),
                    Ranking = table.Column<long>(nullable: false),
                    Score = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ranks", x => x.Gamertag);
                });

            migrationBuilder.CreateTable(
                name: "Scores",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CustomTag = table.Column<string>(maxLength: 50, nullable: true),
                    DateAchieved = table.Column<DateTime>(nullable: false),
                    Gamertag = table.Column<string>(maxLength: 50, nullable: false),
                    Score = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scores", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Scores_DateAchieved_Gamertag_Score",
                table: "Scores",
                columns: new[] { "DateAchieved", "Gamertag", "Score" });

                
            migrationBuilder.Sql(@"
CREATE PROCEDURE [dbo].[GetPlayerRank]
	@Gamertag NVARCHAR(50),
	@Rank INT OUTPUT
AS
SET @Rank = -1

SELECT
	@Rank = Ranking
FROM (
	SELECT
		Gamertag,
		MAX(Score) AS Score,
		MAX(CustomTag) AS CustomTag,
		RANK() OVER (ORDER BY MAX(Score) DESC) AS Ranking
	FROM Scores
	GROUP BY Gamertag
) AS T
WHERE Gamertag = @Gamertag

RETURN 0
");

            migrationBuilder.Sql(@"
CREATE PROCEDURE [dbo].[GetHighScores]
	@StartRank int = 0,
	@Count int
AS
SELECT
	Score,
	Gamertag,
	CustomTag,
	Ranking
FROM
	(SELECT
		Score,
		Gamertag,
		CustomTag,
		RANK() OVER(ORDER BY Score DESC) AS Ranking
		FROM (
			SELECT
				Gamertag,
				MAX(Score) AS Score,
				MAX(CustomTag) AS CustomTag
			FROM Scores
			GROUP BY GamerTag
		) AS T
	) AS T2
WHERE Ranking BETWEEN @StartRank AND (@StartRank + @Count)
ORDER BY Ranking, GamerTag
RETURN 0
");


            migrationBuilder.Sql(@"
CREATE PROCEDURE[dbo].[GetScoresAroundPlayer]

    @Gamertag NVARCHAR(50),
	@Radius INT
AS
DECLARE @PlayerRank int

EXEC GetPlayerRank @Gamertag, @PlayerRank OUTPUT

IF(@PlayerRank >= 0)
BEGIN
    SELECT

        Gamertag,
        Score,
        CustomTag,
        Ranking

    FROM(
        SELECT

            Gamertag,
            MAX(Score) AS Score,
            MAX(CustomTag) AS CustomTag,
            RANK() OVER(ORDER BY MAX(Score) DESC) AS Ranking

        FROM Scores

        GROUP BY GamerTag
    ) AS T

    WHERE Ranking BETWEEN(@PlayerRank - @Radius) AND(@PlayerRank + @Radius)

    ORDER BY Ranking, GamerTag
END
ELSE
BEGIN
    SELECT
        Gamertag = NULL,
        Score = NULL,
        CustomTag = NULL,
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
