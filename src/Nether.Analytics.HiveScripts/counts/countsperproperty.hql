DROP TABLE IF EXISTS countsperproperty;

CREATE TABLE IF NOT EXISTS countsperproperty(
    eventDate DATE,
    displayName STRING,
    propertyKey STRING,
    propertyValue STRING,
    totalCount BIGINT
)
COMMENT ''
PARTITIONED BY (year INT, month INT, day INT)
ROW FORMAT DELIMITED
    FIELDS TERMINATED BY '|'
    LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:countsperproperty}';


INSERT INTO TABLE countsperproperty
PARTITION (year, month, day)
SELECT
    eventDate,
    displayName,
    propertyKey,
    propertyValue,
    sum(value) as totalCount,
    year,
    month,
    day
FROM
    strippedcounts
LATERAL VIEW explode(properties) propertyTable AS propertyKey, propertyValue
GROUP BY 
    eventDate, propertyKey, propertyValue, displayname;