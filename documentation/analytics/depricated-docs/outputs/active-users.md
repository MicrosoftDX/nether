## This documentation is depricated and will be replaced

# Active Users: DAU, MAU

Total number of distinct active users in a given time unit, e.g. day, month, year.

## Output folder in Blob Storage

Container: results
Folder: activeusers
Files:

* dau.csv
* mau.csv

## Output table in SQL

DailyActiveUsers
MonthlyActiveUsers

## Output format

Columns:

* Date without time, e.g. 2017-02-14
  * for month: always the first date of the month, e.g. 2017-02-01 for 2017-02-14
* Total number of distinct active users