DROP TABLE IF EXISTS levelDropoutDistribution;

CREATE EXTERNAL TABLE IF NOT EXISTS levelDropoutDistribution(
    eventDate DATE,
    level INT,
    totalCount BIGINT
)
COMMENT 'distribution of achieved levels per day'
ROW FORMAT DELIMITED
    FIELDS TERMINATED BY '|'
    LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:levelDropoutLoc}';

INSERT INTO TABLE levelDropoutDistribution
SELECT
    eventDate,
    reachedLevel as level,
    count(*) as totalCount
FROM
    levelreached
GROUP BY
    eventDate, reachedLevel;