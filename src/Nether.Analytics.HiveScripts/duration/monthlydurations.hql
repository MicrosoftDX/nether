DROP TABLE IF EXISTS monthlydurations;

CREATE TABLE IF NOT EXISTS monthlydurations(
    eventMonth DATE,
    displayName STRING,
    avgduration BIGINT
)
COMMENT 'average durations per display name per month'
ROW FORMAT DELIMITED
    FIELDS TERMINATED BY '|'
    LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:monthlydurations}';


INSERT INTO TABLE monthlydurations
SELECT
    to_date(eventMonth) AS eventMonth,
    displayName,
    avg(timeSpanSeconds) as avgduration
FROM
    durations
GROUP BY
    eventMonth, displayName;