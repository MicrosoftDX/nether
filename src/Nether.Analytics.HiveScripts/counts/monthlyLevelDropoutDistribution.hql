DROP TABLE IF EXISTS monthlyLevelDropoutDistribution;

CREATE EXTERNAL TABLE IF NOT EXISTS monthlyLevelDropoutDistribution(
    evenMonth DATE,
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
    eventMonth,
    reachedLevel as level,
    count(*) as totalCount
FROM
    levelreached
GROUP BY
    evenMonth, reachedLevel;