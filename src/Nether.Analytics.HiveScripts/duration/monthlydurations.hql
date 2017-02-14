DROP TABLE IF EXISTS monthlydurations;

CREATE TABLE IF NOT EXISTS monthlydurations(
    eventDate DATE,
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
    trunc(eventDate,'MM') as eventDate,
    displayName,
    avg(timeSpanSeconds) as avgduration
FROM
    durations
GROUP BY
    year, month, displayName;