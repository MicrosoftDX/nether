set mapred.reduce.tasks=1;

DROP TABLE IF EXISTS monthlyactivesessions;

CREATE TABLE monthlyactivesessions(
    eventMonth DATE,
    activeSessions INT
)
ROW FORMAT DELIMITED
        FIELDS TERMINATED BY '|'
        LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:monthlyactivesessions}';


INSERT INTO TABLE monthlyactivesessions
SELECT
    eventMonth,
    count(DISTINCT(gameSessionId)) as activeSessions
FROM
    rawgamedurations
GROUP BY
    eventMonth;
