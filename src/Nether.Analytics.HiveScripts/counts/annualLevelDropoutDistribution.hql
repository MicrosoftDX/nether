DROP TABLE IF EXISTS annualLevelDropoutDistribution;

CREATE EXTERNAL TABLE IF NOT EXISTS annualLevelDropoutDistribution(
    year INT,
    level INT,
    totalCount BIGINT
)
COMMENT 'distribution of achieved levels per day'
ROW FORMAT DELIMITED
    FIELDS TERMINATED BY '|'
    LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:annualLevelDropoutLoc}';

INSERT INTO TABLE annualLevelDropoutDistribution
SELECT
    year,
    reachedLevel as level,
    count(*) as totalCount
FROM
    levelreached
GROUP BY
    year, reachedLevel;