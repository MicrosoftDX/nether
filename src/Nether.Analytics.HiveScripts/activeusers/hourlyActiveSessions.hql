set mapred.reduce.tasks=1;

DROP TABLE IF EXISTS hourlyactivesessions;

CREATE TABLE hourlyactivesessions(
    eventDate DATE,
    hour INT,
    activeSessions INT
)
ROW FORMAT DELIMITED
        FIELDS TERMINATED BY '|'
        LINES TERMINATED BY '\n'
STORED AS TEXTFILE
location 'wasbs://intermediate@oknether.blob.core.windows.net/activesessions/hourly/';
--LOCATION '${hiveconf:hourlyactivesessions}';


INSERT INTO TABLE hourlyactivesessions
SELECT
    eventDate,
    hour,
    count(DISTINCT(gameSessionId)) as activeSessions
FROM
    rawgamedurations
GROUP BY
    eventDate, hour;
