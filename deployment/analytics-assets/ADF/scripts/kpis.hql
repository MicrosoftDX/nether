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
STORED AS TEXTFILE
TBLPROPERTIES("skip.header.line.count"="1");

ALTER TABLE rawevents ADD PARTITION (date='2016-09-28') location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/date=2016-09-28';
ALTER TABLE rawevents ADD PARTITION (date='2016-09-29') location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/date=2016-09-29';
ALTER TABLE rawevents ADD PARTITION (date='2016-09-30') location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/date=2016-09-30';
ALTER TABLE rawevents ADD PARTITION (date='2016-10-01') location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/date=2016-10-01';
ALTER TABLE rawevents ADD PARTITION (date='2016-10-02') location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/date=2016-10-02';
ALTER TABLE rawevents ADD PARTITION (date='2016-10-03') location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/date=2016-10-03';
ALTER TABLE rawevents ADD PARTITION (date='2016-10-04') location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/date=2016-10-04';
ALTER TABLE rawevents ADD PARTITION (date='2016-10-05') location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/date=2016-10-05';
ALTER TABLE rawevents ADD PARTITION (date='2016-10-06') location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/date=2016-10-06';
ALTER TABLE rawevents ADD PARTITION (date='2016-10-07') location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/date=2016-10-07';
ALTER TABLE rawevents ADD PARTITION (date='2016-10-08') location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/date=2016-10-08';
ALTER TABLE rawevents ADD PARTITION (date='2016-10-09') location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/date=2016-10-09';
ALTER TABLE rawevents ADD PARTITION (date='2016-10-10') location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/date=2016-10-10';
ALTER TABLE rawevents ADD PARTITION (date='2016-10-11') location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/date=2016-10-11';
ALTER TABLE rawevents ADD PARTITION (date='2016-10-12') location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/date=2016-10-12';
ALTER TABLE rawevents ADD PARTITION (date='2016-10-13') location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/date=2016-10-13';
ALTER TABLE rawevents ADD PARTITION (date='2016-11-18') location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/date=2016-11-18';
ALTER TABLE rawevents ADD PARTITION (date='2016-11-19') location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/date=2016-11-19';
ALTER TABLE rawevents ADD PARTITION (date='2016-11-20') location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/date=2016-11-20';
ALTER TABLE rawevents ADD PARTITION (date='2016-11-21') location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/date=2016-11-21';



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
drop table if exists dailyactivesessions;
CREATE EXTERNAL TABLE dailyactivesessions
(
  	eventdate DATE,
    DAU INT
)
stored as textfile location '${hiveconf:nddau}';
insert OVERWRITE TABLE dailyactivesessions
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