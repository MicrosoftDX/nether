DROP TABLE IF EXISTS dailyLevelDropoutDistribution;

CREATE EXTERNAL TABLE IF NOT EXISTS dailyLevelDropoutDistribution(
    eventDate DATE,
    level INT,
    totalCount BIGINT
)
COMMENT 'distribution of achieved levels per day'
ROW FORMAT DELIMITED
    FIELDS TERMINATED BY '|'
    LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:dailyLevelDropoutLoc}';

INSERT INTO TABLE dailyLevelDropoutDistribution
SELECT
    eventDate,
    reachedLevel as level,
    count(*) as totalCount
FROM
    levelreached
GROUP BY
    eventDate, reachedLevel;