DROP TABLE IF EXISTS agghourlystorecounts;
DROP TABLE IF EXISTS aggdailystorecounts;
DROP TABLE IF EXISTS aggmonthlystorecounts;
DROP TABLE IF EXISTS aggyearlystorecounts;


CREATE TABLE IF NOT EXISTS agghourlystorecounts(
    hour INT,
    itemBought STRING,
    sumamount BIGINT
)
COMMENT 'total count of itenBought per game session per hour'
PARTITIONED BY (year INT, month INT, day INT)
ROW FORMAT DELIMITED
    FIELDS TERMINATED BY '|'
    LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:agghourlystorecounts}';


CREATE TABLE IF NOT EXISTS aggdailystorecounts(
    day INT,
    itemBought STRING,
    sumamount BIGINT
)
COMMENT 'total count per itenBought per game session per day'
PARTITIONED BY (year INT, month INT)
ROW FORMAT DELIMITED
    FIELDS TERMINATED BY '|'
    LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:aggdailystorecounts}';


CREATE TABLE IF NOT EXISTS aggmonthlystorecounts(
    month INT,
    itemBought STRING,
    sumamount BIGINT
)
COMMENT 'total count per itenBought per game session per month'
PARTITIONED BY (year INT)
ROW FORMAT DELIMITED
    FIELDS TERMINATED BY '|'
    LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:aggmonthlystorecounts}';


CREATE TABLE IF NOT EXISTS aggyearlystorecounts(
    year INT,
    itemBought STRING,
    sumamount BIGINT
)
COMMENT 'total count per itenBought per game session per year'
ROW FORMAT DELIMITED
    FIELDS TERMINATED BY '|'
    LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:aggyearlystorecounts}';


INSERT INTO TABLE agghourlystorecounts
PARTITION (year, month, day)
SELECT
    hour,
    itemBought,
    sum(value),
    year,
    month,
    day
FROM
    strippedstorecounts
GROUP BY
    year, month, day, hour, itemBought;


INSERT INTO TABLE aggdailystorecounts
PARTITION (year, month)
SELECT
    day,
    itemBought,
    sum(value),
    year,
    month
FROM
    strippedstorecounts
GROUP BY
    year, month, day, itemBought;


INSERT INTO TABLE aggmonthlystorecounts
PARTITION (year)
SELECT
    month,
    itemBought,
    sum(value),
    year
FROM
    strippedstorecounts
GROUP BY
    year, month, itemBought;


INSERT INTO TABLE aggyearlystorecounts
SELECT
    year,
    itemBought,
    sum(value)
FROM
    strippedstorecounts
GROUP BY
    year, itemBought;