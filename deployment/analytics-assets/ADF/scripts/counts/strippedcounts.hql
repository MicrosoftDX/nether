CREATE EXTERNAL TABLE IF NOT EXISTS counts(event string COMMENT 'event type',
       version string,
       clientutc timestamp,
       displayName string,
       value BIGINT,
       gameSessionId string,
       tags ARRAY<STRING>)
COMMENT 'generic count events'
ROW FORMAT DELIMITED
        FIELDS TERMINATED BY '|'
        COLLECTION ITEMS TERMINATED BY '\073'
        LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:counteventsloc}';


DROP TABLE IF EXISTS strippedcounts;

CREATE TABLE IF NOT EXISTS strippedcounts(
    year INT,
    month INT,
    day INT,
    hour INT,
    displayName STRING,
    value BIGINT,
    gameSessionId STRING,
    tags ARRAY<STRING>
)
COMMENT 'counts in a stripped format'
ROW FORMAT DELIMITED
        FIELDS TERMINATED BY '|'
        COLLECTION ITEMS TERMINATED BY '\073'
        LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:strippedcounts}';


INSERT INTO TABLE strippedcounts
SELECT
    year(clientUtc) as year,
    month(clientUtc) as month,
    day(clientUtc) as day,
    hour(clientUtc) as hour,
    displayName,
    value,
    gameSessionId,
    tags
FROM
    counts;