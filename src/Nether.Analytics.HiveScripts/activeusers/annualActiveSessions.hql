DROP TABLE IF EXISTS annualactivesessions;

CREATE TABLE annualactivesessions(
    year INT,
    activeSessions INT
)
ROW FORMAT DELIMITED
        FIELDS TERMINATED BY '|'
        LINES TERMINATED BY '\n'
STORED AS TEXTFILE
location 'wasbs://intermediate@oknether.blob.core.windows.net/activesessions/annually/';
--LOCATION '${hiveconf:annualactivesessions}';


INSERT INTO TABLE annualactivesessions
SELECT
    year,
    count(DISTINCT(gameSessionId)) as activeSessions
FROM
    rawgamedurations
GROUP BY
    year;
