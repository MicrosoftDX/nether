set mapred.reduce.tasks=1;

DROP TABLE IF EXISTS dailyactiveusers;

CREATE TABLE dailyactiveusers(
    eventDate DATE,
    activeUsers INT
)
ROW FORMAT DELIMITED
        FIELDS TERMINATED BY '|'
        LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:dau}';


INSERT INTO TABLE dailyactiveusers
SELECT
    eventDate,
    count(DISTINCT(gamerTag)) as activeUsers
FROM
    rawgamedurations
GROUP BY
    eventDate;
