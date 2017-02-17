/* Copyright (c) Microsoft. All rights reserved.
Licensed under the MIT license. See LICENSE file in the project root for full license information. */

CREATE PROCEDURE [dbo].GetPlayerRank
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