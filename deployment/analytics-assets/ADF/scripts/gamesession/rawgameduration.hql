DROP TABLE IF EXISTS rawgamedurations;


CREATE TABLE IF NOT EXISTS rawgamedurations(
    startTime TIMESTAMP,
    stopTime TIMESTAMP,
    timeSpanSeconds BIGINT,
    gameSessionId STRING,
    gamerTag STRING,
    lastEventType STRING
)
COMMENT 'raw game session durations'
PARTITIONED BY (year INT, month INT, day INT)
ROW FORMAT DELIMITED
    FIELDS TERMINATED BY '|'
    COLLECTION ITEMS TERMINATED BY '\073'
    LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:rawgamedurations}';


INSERT INTO TABLE rawgamedurations
PARTITION (year, month, day)
SELECT
    gamestart.clientUtc as startTime,
    CASE WHEN isnotnull(gamestop.clientUtc) THEN gamestop.clientUtc ELSE lastgameheartbeat.heartbeatTime END AS stopTime,
    CASE WHEN isnotnull(gamestop.clientUtc) THEN unix_timestamp(gamestop.clientUtc) - unix_timestamp(gamestart.clientUtc) ELSE unix_timestamp(lastgameheartbeat.heartbeatTime) - unix_timestamp(gamestart.clientUtc) END AS timeSpanSeconds,
    gamestart.gameSessionId AS gameSessionId,
    gamestart.gamerTag AS gamerTag,
    CASE WHEN isnotnull(gamestop.clientUtc) THEN 'stop' WHEN isnotnull(lastgameheartbeat.heartbeatTime) THEN 'heartbeat' ELSE 'no-stop-heartbeat' END AS lastEventType,
    year(gamestart.clientUtc) as year,
    month(gamestart.clientUtc) as month,
    day(gamestart.clientUtc) as day
FROM
    gamestart LEFT JOIN lastgameheartbeat ON (gamestart.gameSessionId = lastgameheartbeat.gameSessionId) 
    LEFT JOIN gamestop ON (gamestart.gameSessionId = gamestop.gameSessionId);