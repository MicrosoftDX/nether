DROP TABLE IF EXISTS levelreached;

CREATE TABLE IF NOT EXISTS levelreached(
    eventDate DATE,
    gameSessionId STRING,
    reachedLevel BIGINT
)
COMMENT 'At which level did a given game session end'
PARTITIONED BY (year INT, month INT, day INT)
ROW FORMAT DELIMITED
    FIELDS TERMINATED BY '|'
    LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:levelreached}';


INSERT INTO TABLE levelreached
PARTITION (year, month, day)
SELECT
    eventDate,
    gameSessionId,
    totalCount,
    year,
    month,
    day
FROM
    countNameSessionId
WHERE
    displayName = "level:up";