DROP TABLE IF EXISTS monthlygamedurations;

CREATE TABLE IF NOT EXISTS monthlygamedurations(
    eventMonth DATE,
    avgduration BIGINT
)
COMMENT 'average game session durations per day'
ROW FORMAT DELIMITED
    FIELDS TERMINATED BY '|'
    LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:monthlygamedurations}';


INSERT INTO TABLE monthlygamedurations
SELECT
    to_date(eventMonth) AS eventMonth,
    avg(timeSpanSeconds) as avgduration
FROM
    rawgamedurations
GROUP BY
    eventMonth;