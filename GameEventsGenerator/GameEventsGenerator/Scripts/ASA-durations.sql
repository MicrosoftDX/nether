-- Session durations of each player
WITH Durations AS
(
    SELECT
        PlayerId, GameId, time as EndTime, Latitude, Longitude, City, Country, DATEDIFF(second, LAST(time) OVER (
            PARTITION BY PlayerId, GameId
            LIMIT DURATION(second, 300)
            WHEN GameActivity = '1'
        ), time) as Duration
    FROM
        Input TIMESTAMP BY time
    WHERE
        GameActivity = '0'
)

SELECT GameId, PlayerId, EndTime, Duration AS DurationInSeconds INTO outSQL FROM Durations
SELECT PlayerId as Player, GameId as Game, EndTime, Latitude, Longitude, City, Country, Duration INTO outBlob FROM Durations
SELECT PlayerId as Player, GameId as Game, EndTime, Latitude, Longitude, City, Country, Duration INTO outPBI FROM Durations

-- Player information
SELECT
    PlayerId, Latitude, Longitude, City, Country
INTO
    outSQLgeo
FROM
    Durations
