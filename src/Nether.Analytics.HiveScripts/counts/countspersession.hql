DROP TABLE IF EXISTS countspersession;

CREATE TABLE IF NOT EXISTS countspersession(
    gameSessionId STRING,
    displayName STRING,
    sumCounts BIGINT,
    tags ARRAY<STRING>
)
COMMENT 'counts of displayName per game session'
PARTITIONED BY (year INT, month INT, day INT)
ROW FORMAT DELIMITED
        FIELDS TERMINATED BY '|'
        COLLECTION ITEMS TERMINATED BY '\073'
        LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:countspersession}';

INSERT INTO TABLE countspersession
PARTITION (year, month, day)
SELECT
    gameSessionId,
    displayName,
    sum(value),
    tags,
    year,
    month,
    day
FROM
    strippedcounts
WHERE
    (length(gameSessionId) > 0)
GROUP BY
    year, month, day, gameSessionId, displayName, tags;