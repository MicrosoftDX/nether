DROP TABLE IF EXISTS lastgameheartbeat;

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
    max(struct(gameSessionId, clientUtc)).col2 AS heartbeatTime
FROM
    gameheartbeat
GROUP BY
    gameSessionId;