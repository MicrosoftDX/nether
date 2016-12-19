-- Very raw event data straight from Azure Stream Analytics
CREATE EXTERNAL TABLE IF NOT EXISTS rawevents(event STRING, version STRING, clientUtc STRING, clientyear SMALLINT, clientMonth SMALLINT, clientDay TINYINT, clientHour TINYINT, gamertag STRING)
PARTITIONED BY (year int, month int, day int)
ROW FORMAT DELIMITED FIELDS TERMINATED BY ',' lines terminated by '\n'
STORED AS TEXTFILE
TBLPROPERTIES("skip.header.line.count"="1");

ALTER TABLE rawevents ADD IF NOT EXISTS PARTITION (year='${hiveconf:year}', month='${hiveconf:month}', day='${hiveconf:day}') location '${hiveconf:rawevents}/${hiveconf:year}/${hiveconf:month}/${hiveconf:day}';


-- daily active users
DROP TABLE if exists dailyactiveusers;
CREATE TABLE dailyactiveusers(
  year INT,
  month INT,
  day INT,
  dau INT
)
ROW FORMAT DELIMITED FIELDS TERMINATED BY ',' lines terminated by '\n'
stored as textfile location '${hiveconf:dau}';
INSERT INTO TABLE dailyactiveusers
SELECT
  clientyear as year,
  clientMonth as month,
  clientDay as day,
  count(DISTINCT(gamertag)) as dau
FROM
  rawevents
GROUP BY
  clientyear, clientMonth, clientDay;


-- monthly active users
DROP TABLE if exists montlyactiveusers;
CREATE TABLE montlyactiveusers(
  year INT,
  month INT,
  mau INT
)
ROW FORMAT DELIMITED FIELDS TERMINATED BY ',' lines terminated by '\n'
stored as textfile location '${hiveconf:mau}';
INSERT INTO TABLE montlyactiveusers
SELECT
  clientyear as year,
  clientMonth as month,
  count(DISTINCT(gamertag)) as mau
FROM
  rawevents
GROUP BY
  clientyear, clientMonth;