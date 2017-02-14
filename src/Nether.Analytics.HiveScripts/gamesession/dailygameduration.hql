DROP TABLE IF EXISTS dailygamedurations;

CREATE TABLE IF NOT EXISTS dailygamedurations(
    year INT,
    month INT,
    day INT,
    avgduration BIGINT
)
COMMENT 'average game session durations per day'
ROW FORMAT DELIMITED
    FIELDS TERMINATED BY '|'
    LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:dailygamedurations}';


INSERT INTO TABLE dailygamedurations
SELECT
    year,
    month,
    day,
    avg(timeSpanSeconds) as avgduration
FROM
    rawgamedurations
GROUP BY
    year, month, day;