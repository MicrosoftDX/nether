DROP TABLE IF EXISTS gameheartbeat;
DROP TABLE IF EXISTS lastgameheartbeat;

CREATE EXTERNAL TABLE IF NOT EXISTS gameheartbeat(
    event STRING,
    version STRING,
    enqueueTime TIMESTAMP,
    dequeueTime TIMESTAMP,
    clientUtc TIMESTAMP,
    gameSessionId STRING
)
COMMENT 'Game session heartbeat events'
ROW FORMAT DELIMITED
    FIELDS TERMINATED BY '|'
    COLLECTION ITEMS TERMINATED BY '\073'
    LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:gameheartbeateventsloc}';


CREATE TABLE IF NOT EXISTS lastgameheartbeat
(
    gameSessionId STRING,
    heartbeatTime TIMESTAMP
)
COMMENT 'last heart beat by given game session ID'
ROW FORMAT DELIMITED
    FIELDS TERMINATED BY '|'
    COLLECTION ITEMS TERMINATED BY '\073'
    LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:lastheartbeats}';


INSERT INTO TABLE lastgameheartbeat
SELECT
    gameSessionId,
    max(struct(gameSessionId, enqueueTime)).col2 AS heartbeatTime
FROM
    gameheartbeat
GROUP BY
    gameSessionId;