DROP TABLE IF EXISTS counts;
DROP TABLE IF EXISTS strippedcounts;

CREATE EXTERNAL TABLE IF NOT EXISTS counts(event string COMMENT 'event type',
       version string,
       enqueueTime TIMESTAMP,
       dequeueTime TIMESTAMP,
       clientutc timestamp,
       displayName string,
       value BIGINT,
       gameSessionId string,
       properties MAP<STRING, STRING>)
COMMENT 'generic count events'
ROW FORMAT DELIMITED
        FIELDS TERMINATED BY '|'
        COLLECTION ITEMS TERMINATED BY '\073'
        MAP KEYS TERMINATED BY '='
        LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:counteventsloc}';

CREATE TABLE IF NOT EXISTS strippedcounts(
    eventDate DATE,
    eventMonth STRING,
    hour INT,
    displayName STRING,
    value BIGINT,
    gameSessionId STRING,
    properties MAP<STRING, STRING>
)
COMMENT 'counts in a stripped format'
PARTITIONED BY (year INT, month INT, day INT)
ROW FORMAT DELIMITED
        FIELDS TERMINATED BY '|'
        COLLECTION ITEMS TERMINATED BY '='
        LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:strippedcounts}';


INSERT INTO TABLE strippedcounts
PARTITION (year, month, day)
SELECT
    to_date(enqueueTime) as eventDate,
    if(month(enqueueTime)<10, concat(year(enqueueTime), '-0', month(enqueueTime), '-01'), concat(year(enqueueTime), '-', month(enqueueTime), '-01')) as eventMonth,
    hour(clientUtc) as hour,
    displayName,
    value,
    gameSessionId,
    properties,
    year(enqueueTime) as year,
    month(enqueueTime) as month,
    day(enqueueTime) as day
FROM
    counts;