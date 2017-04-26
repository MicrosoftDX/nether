## This documentation is depricated and will be replaced

# Analytics: Hive Tables

TO DO

## Azure Data Factory

### Active Users / Sessions / Game Sessions

Step | File name | Hive tables created | Comments
---------|----------|---------|---------
 1 | `lastgameheartbeat.hql` | gameheartbeat, lastgameheartbeat | gameheartbeat: raw incoming game heartbeat events. lastgameheartbeat: records the last incoming game heartbeat event per game session id
 2 | `rawgameduration.hql` | gamestart, gamestop, rawgamedurations | gamestart, gamestop: raw incoming game events. rawgamedurations: created in the intermediate container with game stop or game heartbeat if no game stop event has been received
 3 | `dailygameduration.hql` | dailygamedurations | average game session durations per day
 4 | `monthlygameduration.hql` | monthlygamedurations | average game session durations per month
 5 | `yearlygamedurations.hql` | yearlygamedurations | average game session durations per year
 6 | `dau.hql` | dailyactiveusers | average game session durations per day
 7 | `mau.hql` | monthlyactiveusers | average game session durations per month
 8 | `annualActiveUsers.hql` | annualactiveusers | average game session durations per year
 9 | `dailyActiveSessions.hql` | dailyactivesessions | average game session durations per day
 10 | `monthlyActiveSessions.hql` | monthlyactivesessions | average game session durations per month
 11 | `annualActiveSessions.hql` | annualactivesessions | average game session durations per year


### Durations

Step | File name | Hive tables created | Comments
---------|----------|---------|---------
 1 | `durations.hql` | starts, stops, durations | starts, stops: raw incoming game events. durations: created in the intermediate container
 2 | `dailydurations.hql` | dailydurations | average generic durations per day
 3 | `monthlydurations.hql` | monthlydurations | average generic durations per month
 4 | `yearlydurations.hql` | yearlydurations | average generic durations per year


### Counts

Step | File name | Hive tables created | Comments
---------|----------|---------|---------
 1 | `strippedcounts.hql` | counts, strippedcounts | counts: raw incoming count events. strippedcounts: created in the intermediate container
 2 | `countsNameSessionId.hql` | countsNameSessionId | counts per DisplayName per game session
 3 | `levelup.hql` | levelreached | at which level did a given game session end
 4 | `dailyLevelDropoutDistribution.hql` | dailyLevelDropoutDistribution | distribution of achieved levels per day
 5 | `monthlyLevelDropoutDistribution.hql` | monthlyLevelDropoutDistribution | distribution of achieved levels per day
 6 | `annualLevelDropoutDistribution.hql` | annualLevelDropoutDistribution | distribution of achieved levels per day