DROP TABLE IF EXISTS dailydurations;

CREATE TABLE IF NOT EXISTS dailydurations(
    eventDate DATE,
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
    eventDate,
    displayName,
    avg(timeSpanSeconds) as avgduration
FROM
    durations
GROUP BY
    eventDate, displayName;