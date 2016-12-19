SET hive.exec.dynamic.partition=true;
SET hive.exec.dynamic.partition.mode = nonstrict;
set hive.cli.print.header=true;

-- Very raw event data straight from Azure Stream Analytics
DROP TABLE if exists rawevents;
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
partitioned by (year int, month int, day int)
ROW FORMAT DELIMITED FIELDS TERMINATED BY ',' lines terminated by '\n'
STORED AS TEXTFILE
TBLPROPERTIES("skip.header.line.count"="1");

ALTER TABLE rawevents ADD PARTITION (year=2016, month=10, day=01) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/10/01';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=10, day=02) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/10/02';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=10, day=03) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/10/03';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=10, day=04) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/10/04';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=10, day=05) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/10/05';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=10, day=06) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/10/06';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=10, day=07) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/10/07';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=10, day=08) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/10/08';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=10, day=09) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/10/09';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=10, day=10) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/10/10';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=11, day=20) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/11/20';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=11, day=21) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/11/21';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=11, day=22) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/11/22';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=11, day=23) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/11/23';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=11, day=24) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/11/24';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=11, day=25) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/11/25';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=11, day=26) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/11/26';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=11, day=27) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/11/27';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=11, day=28) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/11/28';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=11, day=29) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/11/29';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=12, day=01) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/12/01';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=12, day=02) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/12/02';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=12, day=03) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/12/03';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=12, day=04) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/12/04';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=12, day=05) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/12/05';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=12, day=06) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/12/06';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=12, day=07) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/12/07';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=12, day=08) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/12/08';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=12, day=09) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/12/09';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=12, day=10) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/12/10';


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


DROP TABLE if exists dausers;
CREATE EXTERNAL TABLE dausers(
  year INT,
  month INT,
  day INT,
  dau INT
)
stored as textfile location '${hiveconf:dau}';
INSERT OVERWRITE TABLE 