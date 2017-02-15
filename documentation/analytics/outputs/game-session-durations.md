# Game Session Durations

Game session durations provides the average session lengths per given time unit, e.g. day, month, year.

## Output folder in Blob Storage

Container: results
Folder: gamedurations
Files:
* dailyGameDurations.csv
* monthlyGameDurations.csv
* yearlyGameDurations.csv

## Output table in SQL

DailyGameDurations
MonthlyGameDurations
YearlyGameDurations

## Output format

Delimited by |
Columns:

* Date without time, e.g. 2017-02-14
* Average Duration in Seconds

Examples:
* dailyGameDurations.csv: 2017-02-14|359
* monthlyGameDurations.csv: 2017-02-01|359
* yearlyGameDurations.csv: 2017|359

## Hive Scripts

Folder: [src/Nether.Analytics.HiveScripts/gamesession](src/Nether.Analytics.HiveScripts/gamesession)


File name | Actions 
---------|----------
 lastgameheartbeat.hql | table of last game heartbeat per game session id
 gamedurations.hql | table of session durations with associated start and stop time per game session id
 dailygamedurations.hql | table of average session durations per day
 monthlygamedurations.hql | table of average session durations per month
 yearlygamedurations.hql | table of average session durations per year