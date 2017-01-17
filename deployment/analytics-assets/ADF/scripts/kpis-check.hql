SET mapred.input.dir.recursive=true;
SET hive.mapred.supports.subdirectories=true;

-- Very raw event data straight from Azure Stream Analytics
DROP TABLE rawevents;
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
ROW FORMAT DELIMITED FIELDS TERMINATED BY ',' lines terminated by '\n'
STORED AS TEXTFILE location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/12/'
TBLPROPERTIES("skip.header.line.count"="1");


-- Very raw event data straight from Azure Stream Analytics
DROP TABLE rawevs;
CREATE EXTERNAL TABLE IF NOT EXISTS rawevs(event STRING, version STRING, clientUtc STRING, clientyear SMALLINT, clientMonth SMALLINT, clientDay TINYINT, clientHour TINYINT, gamertag STRING)
PARTITIONED BY (year int, month int, day int)
ROW FORMAT DELIMITED FIELDS TERMINATED BY ',' lines terminated by '\n'
STORED AS TEXTFILE
TBLPROPERTIES("skip.header.line.count"="1");

ALTER TABLE rawevs ADD PARTITION (year=2016, month=12, day=13) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/12/13';
ALTER TABLE rawevs ADD PARTITION (year=2016, month=12, day=12) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/12/12';
ALTER TABLE rawevs ADD PARTITION (year=2016, month=12, day=11) location 'wasbs://gamedata@oldknether.blob.core.windows.net/rawevents/2016/12/11';

ALTER TABLE rawevs DROP PARTITION (year=2016, month=12, day=13);


ALTER TABLE rawevents ADD PARTITION (year=2016, month=12, day=12) location 'wasbs://gameevents@netstuff.blob.core.windows.net/rawevents/2016/12/12';
ALTER TABLE rawevents ADD PARTITION (year=2016, month=12, day=11) location 'wasbs://gameevents@netstuff.blob.core.windows.net/rawevents/2016/12/11';



-- daily active users
DROP TABLE if exists dailyactiveusers;
CREATE TABLE dailyactiveusers( year INT, month INT, day INT, dau INT)
stored as textfile location 'wasbs://gamedata@oldknether.blob.core.windows.net/dailyactiveusers';
INSERT INTO TABLE dailyactiveusers
SELECT year, month, day, count(DISTINCT(gamertag)) as dau
FROM rawevs
GROUP BY year, month, day;


DROP TABLE if exists dailyactiveusers;
CREATE TABLE dailyactiveusers( year INT, month INT, day INT, dau INT)
stored as textfile location 'wasbs://gameevents@netstuff.blob.core.windows.net/dailyactiveusers';
INSERT INTO TABLE dailyactiveusers
SELECT year, month, day, count(DISTINCT(gamertag)) as dau
FROM rawevents
GROUP BY year, month, day;


-- monthly active users
DROP TABLE if exists montlyactiveusers;
CREATE TABLE montlyactiveusers(
  year INT,
  month INT,
  mau INT
)
stored as textfile location '${hiveconf:mau}';
INSERT OVERWRITE TABLE montlyactiveusers
SELECT
  year,
  month,
  count(DISTINCT(gamertag)) as mau
FROM
  rawevents
GROUP BY
  year, month;
