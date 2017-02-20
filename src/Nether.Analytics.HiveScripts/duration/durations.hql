set hive.mapjoin.optimized.hashtable=FALSE;
--Creating all external tables
drop table if EXISTS starts;
drop table if exists stops;
drop table if exists durations;

CREATE EXTERNAL TABLE IF NOT EXISTS starts(event string COMMENT 'event type',
       version string,
       enqueueTime TIMESTAMP,
       dequeueTime TIMESTAMP,
       clientutc timestamp,
       eventCorrelationId string,
       displayName string,
       gameSessionId string,
       tags ARRAY<STRING>)
COMMENT 'generic start events'
ROW FORMAT DELIMITED
        FIELDS TERMINATED BY '|'
        COLLECTION ITEMS TERMINATED BY '\073'
        LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:starteventsloc}';


CREATE EXTERNAL TABLE IF NOT EXISTS stops(event string COMMENT 'event type',
       version string,
       enqueueTime TIMESTAMP,
       dequeueTime TIMESTAMP,
       clientutc timestamp,
       eventCorrelationId string,
       gameSessionId string,
       tags ARRAY<STRING>)
COMMENT 'generic start events'
ROW FORMAT DELIMITED
        FIELDS TERMINATED BY '|'
        COLLECTION ITEMS TERMINATED BY '\073'
        LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:stopeventsloc}';


CREATE TABLE IF NOT EXISTS durations(
    eventDate DATE,
    eventMonth STRING,
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
    to_date(start.enqueueTime) as eventDate,
    if(month(start.enqueueTime)<10, concat(year(start.enqueueTime), '-0', month(start.enqueueTime), '-01'), concat(year(start.enqueueTime), '-', month(start.enqueueTime), '-01')) as eventMonth,
    start.clientUtc as startTime,
    stop.clientUtc as stopTime,
    unix_timestamp(stop.enqueueTime) - unix_timestamp(start.enqueueTime) AS timeSpanSeconds,
    start.eventCorrelationId as eventCorrelationId,
    start.displayName AS displayName,
    start.gameSessionId AS gameSessionId,
    start.tags AS tags,
    year(start.enqueueTime) as year,
    month(start.enqueueTime) as month,
    day(start.enqueueTime) as day
FROM
    starts start JOIN stops stop
ON
    start.eventcorrelationid = stop.eventcorrelationid;