DROP TABLE IF EXISTS monthlyLevelDropoutDistribution;

CREATE EXTERNAL TABLE IF NOT EXISTS monthlyLevelDropoutDistribution(
    eventDate DATE,
    level INT,
    totalCount BIGINT
)
COMMENT 'distribution of achieved levels per day'
ROW FORMAT DELIMITED
    FIELDS TERMINATED BY '|'
    LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:monthlyLevelDropoutLoc}';

INSERT INTO TABLE monthlyLevelDropoutDistribution
SELECT
    eventDate,
    reachedLevel as level,
    count(*) as totalCount
FROM
    levelreached
GROUP BY
    eventDate, reachedLevel;