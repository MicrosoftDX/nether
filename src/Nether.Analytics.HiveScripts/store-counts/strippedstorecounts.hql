DROP TABLE IF EXISTS strippedstorecounts;

CREATE TABLE IF NOT EXISTS strippedstorecounts(
    year INT,
    month INT,
    day INT,
    hour INT,
    itemBought STRING,
    amount BIGINT,
    gameSessionId STRING,
    tags ARRAY<STRING>
)
COMMENT 'store counts in a stripped format'
ROW FORMAT DELIMITED
        FIELDS TERMINATED BY '|'
        COLLECTION ITEMS TERMINATED BY '\073'
        LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:strippedstorecounts}';


INSERT INTO TABLE strippedstorecounts
SELECT
    year(clientUtc) as year,
    month(clientUtc) as month,
    day(clientUtc) as day,
    hour(clientUtc) as hour,
    itemBought,
    amount,
    gameSessionId,
    tags
FROM
    storecounts;
