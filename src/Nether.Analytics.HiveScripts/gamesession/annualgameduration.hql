DROP TABLE IF EXISTS yearlygamedurations;

CREATE TABLE IF NOT EXISTS yearlygamedurations(
    year INT,
    avgduration BIGINT
)
COMMENT 'average game session durations per day'
ROW FORMAT DELIMITED
    FIELDS TERMINATED BY '|'
    LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:yearlygamedurations}';


INSERT INTO TABLE yearlygamedurations
SELECT
    year,
    avg(timeSpanSeconds) as avgduration
FROM
    rawgamedurations
GROUP BY
    year;