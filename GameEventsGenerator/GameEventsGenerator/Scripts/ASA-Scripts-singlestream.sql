WITH ActiveSessions AS
(
    SELECT
        GameId, PlayerId, Country, City, System.timestamp as WindowEnd, Count(*) as NumberOfStarts
    FROM
        Input timestamp by time
    GROUP BY
        HoppingWindow(second, 300, 10), GameId, PlayerId, Country, City
    HAVING
        MIN(GameActivity) = 1
),
Durations AS
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
),
CCU AS
(
    SELECT
        GameId, System.timestamp as timewindow, Country, City, COUNT(PlayerId) as NumActiveSessions
    FROM
        ActiveSessions
    GROUP BY
        GameId, Country, City, SlidingWindow(second, 1)
)


-- Number of active sessions per game ID
SELECT * INTO outBlobCCUs FROM CCU
SELECT * INTO outPBIccu FROM CCU
SELECT GameId, timewindow AS EndTime, NumActiveSessions AS NumberPlayers INTO outSQLccu FROM CCU


-- Session durations of each player
SELECT PlayerId, GameId, EndTime, Duration INTO outBlobDurations FROM Durations
SELECT PlayerId, GameId, EndTime, Duration INTO outPBIdurations FROM Durations
SELECT GameId, PlayerId, EndTime, Duration AS DurationInSeconds INTO outSQLdurations FROM Durations
SELECT PlayerId as Player, GameId as Game, EndTime, Latitude, Longitude, City, Country, Duration INTO outBlobGeo FROM Durations
SELECT PlayerId as Player, GameId as Game, EndTime, Latitude, Longitude, City, Country, Duration INTO outPBIgeo FROM Durations


-- Player information
SELECT
    PlayerId, Latitude, Longitude, City, Country
INTO
    outSQLgeo
FROM
    Durations


--SELECT
--    Make,
--    Time
--FROM
--    Input TIMESTAMP BY Time
--WHERE
--    LAG(Make, 1) OVER (LIMIT DURATION(minute, 1)) <> Make


--------------
-- FOR TEST PURPOSES

-- Simply storing raw data in blob storage as csv and json
Select * into outBlobCsv from Input timestamp by time

Select * into outBlobJson from Input timestamp by time


-- Players joining in 10-second tumbling window
SELECT
    GameId, System.Timestamp AS WindowEnd, COUNT(*) AS Count
INTO
    outBlobJoinings
FROM
    EntryStream TIMESTAMP BY EntryTime
GROUP BY
    TUMBLINGWINDOW(second, 10), GameId


-- Players leaving a game in 10-second tumbling window
SELECT
    GameId, System.Timestamp AS WindowEnd, COUNT(*) AS Count
INTO
    outBlobExitings
FROM
    ExitStream TIMESTAMP BY ExitTime
GROUP BY
    TUMBLINGWINDOW(second, 10), GameId

