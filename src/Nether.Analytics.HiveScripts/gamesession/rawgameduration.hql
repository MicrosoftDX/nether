DROP TABLE IF EXISTS gamestart;
DROP TABLE IF EXISTS gamestop;
DROP TABLE IF EXISTS rawgamedurations;

CREATE EXTERNAL TABLE IF NOT EXISTS gamestart(
    event STRING,
    version STRING,
    enqueueTime TIMESTAMP,
    dequeueTime TIMESTAMP,
    clientUtc TIMESTAMP,
    gameSessionId STRING,
    gamerTag STRING
)
COMMENT 'Game session start events'
ROW FORMAT DELIMITED
    FIELDS TERMINATED BY '|'
    COLLECTION ITEMS TERMINATED BY '\073'
    LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:gamestarteventsloc}';


CREATE EXTERNAL TABLE IF NOT EXISTS gamestop(
    event STRING,
    version STRING,
    enqueueTime TIMESTAMP,
    dequeueTime TIMESTAMP,
    clientUtc TIMESTAMP,
    gameSessionId STRING
)
COMMENT 'Game session stop events'
ROW FORMAT DELIMITED
    FIELDS TERMINATED BY '|'
    COLLECTION ITEMS TERMINATED BY '\073'
    LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:gamestopeventsloc}';


CREATE TABLE IF NOT EXISTS rawgamedurations(
    eventDate DATE,
    eventMonth STRING,
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
    to_date(gamestart.enqueueTime) as eventDate,
    if(month(gamestart.enqueueTime)<10, concat(year(gamestart.enqueueTime), '-0', month(gamestart.enqueueTime), '-01'), concat(year(gamestart.enqueueTime), '-', month(gamestart.enqueueTime), '-01')) as eventMonth,
    gamestart.clientUtc as startTime,
    CASE WHEN isnotnull(gamestop.enqueueTime) THEN gamestop.clientUtc ELSE lastgameheartbeat.heartbeatTime END AS stopTime,
    CASE WHEN isnotnull(gamestop.enqueueTime) THEN unix_timestamp(gamestop.enqueueTime) - unix_timestamp(gamestart.enqueueTime) ELSE unix_timestamp(lastgameheartbeat.heartbeatTime) - unix_timestamp(gamestart.enqueueTime) END AS timeSpanSeconds,
    gamestart.gameSessionId AS gameSessionId,
    gamestart.gamerTag AS gamerTag,
    CASE WHEN isnotnull(gamestop.enqueueTime) THEN 'stop' WHEN isnotnull(lastgameheartbeat.heartbeatTime) THEN 'heartbeat' ELSE 'no-stop-heartbeat' END AS lastEventType,
    year(gamestart.enqueueTime) as year,
    month(gamestart.enqueueTime) as month,
    day(gamestart.enqueueTime) as day
FROM
    gamestart LEFT JOIN lastgameheartbeat ON (gamestart.gameSessionId = lastgameheartbeat.gameSessionId) 
    LEFT JOIN gamestop ON (gamestart.gameSessionId = gamestop.gameSessionId);