DROP TABLE IF EXISTS yearlydurations;

CREATE TABLE IF NOT EXISTS yearlydurations(
    year INT,
    displayName STRING,
    avgduration BIGINT
)
COMMENT 'average durations per display name per month'
ROW FORMAT DELIMITED
    FIELDS TERMINATED BY '|'
    LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:yearlydurations}';


INSERT INTO TABLE yearlydurations
SELECT
    year,
    displayName,
    avg(timeSpanSeconds) as avgduration
FROM
    durations
GROUP BY
    year, displayName;