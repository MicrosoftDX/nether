# Running Test

## Simple Load testing

A sample load testing project allows to measure an average response time per player per call and is located under `src/PerformanceTests/LeaderboardLoadTest`. In order to run the test make sure the server is running first, then navigate to load testing project and run `dotnet run <number-of-users> <number-of-sessions-per-user>` where

- **number of users** specifies a number of simultaneous users playing
- **number of sessions per user** specifies how many playing sessions to simulate

At the end of the exectuion a sample report with perfomance results will show in the terminal:

```
> dotnet run 2.4

load testing with 2 users, 4 calls per each...
waiting for load session to finish...         
Statistics:                                   
                                              
Player 00000                                  
  PostScore                                   
    average call duration: 30.5ms             
    calls/second: 32.7868852459016            
  GetScores                                   
    average call duration: 21ms               
    calls/second: 47.6190476190476            
                                              
Player 00001                                  
  PostScore                                   
    average call duration: 31ms               
    calls/second: 32.258064516129             
  GetScores                                   
    average call duration: 23.5ms             
    calls/second: 42.5531914893617            
                                              
total averages:                               
  PostScore                                   
    average call duration: 30.75ms            
    calls/second: 32.5224748810153            
  GetScores                                   
    average call duration: 22.25ms            
    calls/second: 45.0861195542047            
finished in 00:00:40.2368265                  
```