-- DEPRECATED

-- Session durations of each player
SELECT
    CAST(EntryStream.GameId AS BIGINT) as GameId, CAST(EntryStream.EntryTime AS DATETIME) AS EntryTime, CAST(ExitStream.ExitTime AS DATETIME) AS ExitTime, CAST(EntryStream.PlayerId AS NVARCHAR(MAX)) as PlayerId, CAST(DATEDIFF(second, EntryStream.EntryTime, ExitStream.ExitTime) AS BIGINT) AS DurationInSeconds
INTO
    OutputBlob
FROM
    EntryStream TIMESTAMP BY EntryTime
    JOIN ExitStream TIMESTAMP BY ExitTime
ON
    (EntryStream.GameId = ExitStream.GameId AND EntryStream.PlayerId = ExitStream.PlayerId)
    AND DATEDIFF(second, EntryStream, ExitStream) BETWEEN 1 AND 400


-- Players joining or leaving a game in 10-second tumbling window
With Input AS
(
    SELECT
        GameId, IsStart = 1
    FROM
        EntryStream
UNION
    SELECT
        GameId, IsStart = 0
    FROM
        ExitStream
)
SELECT
    GameId, System.Timestamp AS WindowEnd,
    SUM(CASE WHEN IsStart = 1 THEN 1 ELSE 0 END) as NumberOfStarts,
    SUM(CASE WHEN IsStart = 0 THEN 1 ELSE 0 END) as NumberOfStops
FROM
    Input TIMESTAMP BY time
GROUP BY
    TUMBLINGWINDOW(second, 10), GameId


-- Number of active sessions
WITH ActiveSessions AS
(
    SELECT
        GameId, System.timestamp as WindowEnd,
        SUM(CASE WHEN GameActivity = 'start' THEN 1 ELSE 0 END) as NumberOfStarts,
        SUM(CASE WHEN GameActivity = 'stop' THEN 1 ELSE 0 END) as NumberOfStops
    FROM
        Input timestamp by time
    GROUP BY
        HoppingWindow(second, 400, 10), GameId
)
SELECT
    GameId, System.timestamp as timewindow, SUM(NumberOfStarts - NumberOfStops) as NumActiveSessions
FROM
    ActiveSessions
GROUP BY
    GameId, SlidingWindow(second, 1)


--------------
-- FOR TEST PURPOSES

-- Simply storing raw data in blob storage as csv and json
Select * into outputEntry from EntryStream timestamp by EntryTime

Select * into outEntryJson from EntryStream timestamp by EntryTime

Select * into outputExit from ExitStream timestamp by ExitTime

Select * into outExitJson from ExitStream timestamp by ExitTime


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

