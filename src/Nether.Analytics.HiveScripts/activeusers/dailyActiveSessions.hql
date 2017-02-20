set mapred.reduce.tasks=1;

DROP TABLE IF EXISTS dailyactivesessions;

CREATE TABLE dailyactivesessions(
    eventDate DATE,
    activeSessions INT
)
ROW FORMAT DELIMITED
        FIELDS TERMINATED BY '|'
        LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:dailyactivesessions}';


INSERT INTO TABLE dailyactivesessions
SELECT
    eventDate,
    count(DISTINCT(gameSessionId)) as activeSessions
FROM
    rawgamedurations
GROUP BY
    eventDate;
