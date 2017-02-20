DROP TABLE IF EXISTS countsNameSessionId;

CREATE TABLE IF NOT EXISTS countsNameSessionId(
    eventDate DATE,
    displayName STRING,
    gameSessionId STRING,
    totalCount BIGINT,
    properties MAP<STRING, STRING>
)
COMMENT 'counts per DisplayName per game session'
PARTITIONED BY (year INT, month INT, day INT)
ROW FORMAT DELIMITED
    FIELDS TERMINATED BY '|'
    MAP KEYS TERMINATED BY '='
    LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:countsNameSessionId}';


INSERT INTO TABLE countsNameSessionId
PARTITION (year, month, day)
SELECT
    eventDate,
    displayName,
    gameSessionId,
    sum(value) + 1 as totalCount,
    properties,
    year,
    month,
    day
FROM
    strippedcounts
GROUP BY 
    year, month, day, eventDate, displayName, gameSessionId, properties;