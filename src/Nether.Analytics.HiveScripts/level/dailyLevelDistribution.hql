DROP TABLE IF EXISTS dailyleveldistr;

CREATE TABLE IF NOT EXISTS dailyleveldistr(
    eventDate DATE,
    level INT,
    count BIGINT
)
COMMENT 'distribution of achieved levels per day'
ROW FORMAT DELIMITED
    FIELDS TERMINATED BY '|'
    LINES TERMINATED BY '\n'
STORED AS TEXTFILE
LOCATION '${hiveconf:dailyleveldistribution}';

INSERT INTO TABLE dailyleveldistr
SELECT
    eventDate,
    level,
    sum() as avgduration
FROM
    levelsachieved
GROUP BY
    eventDate, level;