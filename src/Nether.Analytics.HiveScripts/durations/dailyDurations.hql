DROP TABLE IF EXISTS dailydurations;

CREATE TABLE IF NOT EXISTS dailydurations(
    year INT,
    month INT,
    day INT,
    displayName STRING,
    avgduration BIGINT
)
COMMENT 'average durations per display name per day'
ROW FORMAT DELIMITED
    FIELDS TERMINATED BY '|'
    LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:dailydurations}';

INSERT INTO TABLE dailydurations
SELECT
    year,
    month,
    day,
    displayName,
    avg(timeSpanSeconds) as avgduration
FROM
    durations
GROUP BY
    year, month, day, displayName;