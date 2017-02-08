DROP TABLE IF EXISTS countspertags;

CREATE TABLE IF NOT EXISTS countspertags(
    tag STRING,
    displayName STRING,
    totalCount BIGINT
)
COMMENT 'counts of displayName per tag'
PARTITIONED BY (year INT, month INT, day INT)
ROW FORMAT DELIMITED
    FIELDS TERMINATED BY '|'
    LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:countspertags}';


INSERT INTO TABLE countspertags
PARTITION (year, month, day)
SELECT
    tag,
    displayName,
    sum(value) as totalCount,
    year,
    month,
    day
FROM
    strippedcounts
LATERAL VIEW explode(tags) tagTable AS tag
GROUP BY 
    year, month, day, tag, displayname;