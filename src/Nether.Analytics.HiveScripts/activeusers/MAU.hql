DROP TABLE IF EXISTS monthlyactiveusers;

CREATE TABLE monthlyactiveusers(
    eventMonth DATE,
    activeUsers INT
)
ROW FORMAT DELIMITED
        FIELDS TERMINATED BY '|'
        LINES TERMINATED BY '\n'
STORED AS TEXTFILE
location 'wasbs://intermediate@oknether.blob.core.windows.net/activeusers/monthly/';
--LOCATION '${hiveconf:mau}';


INSERT INTO TABLE monthlyactiveusers
SELECT
    eventMonth,
    count(DISTINCT(gamerTag)) as activeUsers
FROM
    rawgamedurations
GROUP BY
    eventMonth;
