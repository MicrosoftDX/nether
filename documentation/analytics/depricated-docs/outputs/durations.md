## This documentation is depricated and will be replaced

# Generic Durations

Generic durations provides the average session lengths per given time unit, e.g. day, month, year.

## Output folder in Blob Storage

Container: results
Folder: durations
Files:

* dailyDurations.csv
* monthlyDurations.csv
* yearlyDurations.csv


## Output table in SQL

* DailyDurations
* MonthlyDurations
* YearlyDurations

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

Folder: [src/Nether.Analytics.HiveScripts/duration](src/Nether.Analytics.HiveScripts/duration)


File name | Actions 
---------|----------
 durations.hql | table of session durations with associated start and stop time using the event correlation id and display name
 dailyDurations.hql | table of average session durations per day per display name
 monthlyDurations.hql | table of average session durations per month per display name
 yearlyDurations.hql | table of average session durations per year per display name