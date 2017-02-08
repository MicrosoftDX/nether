DROP TABLE IF EXISTS default.durations;


CREATE TABLE IF NOT EXISTS default.durations(
    startTime TIMESTAMP,
    stopTime TIMESTAMP,
    timeSpanSeconds BIGINT,
    eventCorrelationId STRING,
    displayName STRING,
    gameSessionId STRING,
    tags ARRAY<STRING>
)
COMMENT 'raw session durations'
PARTITIONED BY (year INT, month INT, day INT)
ROW FORMAT DELIMITED
    FIELDS TERMINATED BY '|'
    COLLECTION ITEMS TERMINATED BY '\073'
    LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:rawdurationsloc}';

INSERT INTO TABLE durations
PARTITION (year, month, day)
SELECT
    start.clientUtc as startTime,
    stop.clientUtc as stopTime,
    unix_timestamp(stop.clientUtc) - unix_timestamp(start.clientUtc) AS timeSpanSeconds,
    start.eventCorrelationId as eventCorrelationId,
    start.displayName AS displayName,
    start.gameSessionId AS gameSessionId,
    start.tags AS tags,
    year(start.clientUtc) as year,
    month(start.clientUtc) as month,
    day(start.clientUtc) as day
FROM
    starts start JOIN stops stop
ON
    start.eventcorrelationid = stop.eventcorrelationid;