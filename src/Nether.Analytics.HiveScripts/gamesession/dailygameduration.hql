DROP TABLE IF EXISTS dailygamedurations;

CREATE TABLE IF NOT EXISTS dailygamedurations(
    eventDate DATE,
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
    eventDate,
    avg(timeSpanSeconds) as avgduration
FROM
    rawgamedurations
GROUP BY
    eventDate;