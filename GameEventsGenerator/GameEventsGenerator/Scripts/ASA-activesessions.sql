-- Number of active sessions per game ID

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
CCU AS
(
    SELECT
        GameId, System.timestamp as timewindow, Country, City, COUNT(PlayerId) as NumActiveSessions
    FROM
        ActiveSessions
    GROUP BY
        GameId, Country, City, SlidingWindow(second, 1)
)


SELECT * INTO outBlobCCUs FROM CCU
SELECT * INTO outPBI FROM CCU
SELECT GameId, timewindow AS EndTime, NumActiveSessions AS NumberPlayers INTO outSQL FROM CCU
