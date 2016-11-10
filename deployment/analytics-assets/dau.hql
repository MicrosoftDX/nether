SET hive.exec.dynamic.partition=true;
SET hive.exec.dynamic.partition.mode = nonstrict;
set hive.cli.print.header=true;

-- Very raw event data straight from Azure Stream Analytics
drop table if exists rawevents;
CREATE EXTERNAL TABLE rawevents(
    event STRING,
    version STRING,
  	clientUtc STRING,
    clientyear SMALLINT,
  	clientMonth SMALLINT,
  	clientDay TINYINT,
  	clientHour TINYINT,
    gamertag STRING
)
partitioned by (`date` string)
ROW FORMAT DELIMITED FIELDS TERMINATED BY ',' lines terminated by '\n'
STORED AS TEXTFILE LOCATION '${hiveconf:rawevents}'
TBLPROPERTIES("skip.header.line.count"="1");

MSCK REPAIR TABLE rawevents;


-- Extending the raw information with two columns for timestamp and date - format readable for Hive
drop table if exists rawinfo;
create external table rawinfo(
  event string,
  version string,
  eventtime TIMESTAMP,
  eventdate DATE,
  eventYear SMALLINT,
  eventMonth SMALLINT,
  eventDay TINYINT,
  eventHour TINYINT,
  gamertag string
  )
stored as textfile location '${hiveconf:rawinfo}';
INSERT OVERWRITE TABLE rawinfo
SELECT
  event,
  version,
  concat(substr(clientUtc,1,10), ' ', substr(clientUtc,12,10)) as eventtime,
  substr(clientUtc,1,10) as eventdate,
  clientyear as eventYear,
  clientMonth as eventMonth,
  clientDay as eventDay,
  clientHour as eventHour,
  gamertag
FROM
  rawevents;


-- Daily active users 
drop table if exists dailyactiveusers;
CREATE EXTERNAL TABLE dailyactiveusers
(
  	eventdate DATE,
    DAU INT
)
stored as textfile location '${hiveconf:dau}';
insert OVERWRITE TABLE dailyactiveusers
SELECT
  eventdate,
  COUNT(DISTINCT(gamertag)) AS DAU
FROM
    rawinfo
GROUP BY
    eventdate;


-- Daily active users (non-distinct)
drop table if exists nondailyactiveusers;
CREATE EXTERNAL TABLE nondailyactiveusers
(
  	eventdate DATE,
    DAU INT
)
stored as textfile location '${hiveconf:nddau}';
insert OVERWRITE TABLE nondailyactiveusers
SELECT
  eventdate,
  COUNT(DISTINCT(gamertag)) AS DAU
FROM
    rawinfo
GROUP BY
    eventdate;




-- Monthly active users
drop table if exists monthlyactiveusers;
CREATE EXTERNAL TABLE monthlyactiveusers
(
    year SMALLINT,
  	month SMALLINT,
    MAU INT
)
stored as textfile location '${hiveconf:mau}';
insert OVERWRITE TABLE monthlyactiveusers
SELECT
  eventYear as year,
  eventMonth as month,
  COUNT(DISTINCT(gamertag)) AS MAU
FROM
    rawinfo
GROUP BY
  eventYear, eventMonth;


-- Average session length


-- concurrent users