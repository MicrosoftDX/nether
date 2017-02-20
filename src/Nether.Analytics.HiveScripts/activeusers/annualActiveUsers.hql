DROP TABLE IF EXISTS annualactiveusers;

CREATE TABLE annualactiveusers(
    year INT,
    activeUsers INT
)
ROW FORMAT DELIMITED
        FIELDS TERMINATED BY '|'
        LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:annualactiveusers}';


INSERT INTO TABLE annualactiveusers
SELECT
    year,
    count(DISTINCT(gamerTag)) as activeUsers
FROM
    rawgamedurations
GROUP BY
    year;
