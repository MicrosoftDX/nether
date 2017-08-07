# Analytics Results API

All these APIs require `admin` role

* [Active sessions](#active-sessions)
* [Active users](#active-users)
* [Durations](#durations)
* [Game durations](#game-durations)
* [Level drop-offs](#level-drop-offs)

## Active sessions

Get aggregated active sessions statistics by day, month or year


### Daily

```
    GET /api/analytics/active-sessions/daily
```


### Response: 200 OK

```json
{
  "activeSessions": [
    {
      "year": 2017,
      "month": 2,
      "day": 10,
      "activeSessions": 123
    },
  ]
}
```

### Monthly

```
    GET /api/analytics/active-sessions/monthly
```


### Response: 200 OK

```json
{
  "activeSessions": [
    {
      "year": 2017,
      "month": 2,
      "activeSessions": 123
    },
  ]
}
```



### Yearly

```
    GET /api/analytics/active-sessions/yearly
```


### Response: 200 OK

```json
{
  "activeSessions": [
    {
      "year": 2017,
      "activeSessions": 123
    },
  ]
}
```




## Active users

Get aggregated active users statistics by day, month or year


### Daily

```
    GET /api/analytics/active-users/daily
```


### Response: 200 OK

```json
{
  "activeSessions": [
    {
      "year": 2017,
      "month": 2,
      "day": 10,
      "activeUsers": 123
    },
  ]
}
```

### Monthly

```
    GET /api/analytics/active-users/monthly
```


### Response: 200 OK

```json
{
  "activeSessions": [
    {
      "year": 2017,
      "month": 2,
      "activeUsers": 123
    },
  ]
}
```



### Yearly

```
    GET /api/analytics/active-users/yearly
```


### Response: 200 OK

```json
{
  "activeSessions": [
    {
      "year": 2017,
      "activeUsers": 123
    },
  ]
}
```



## Durations

Get aggregated durations statistics by day, month or year

Durations endpoints take a `name` parameter that corresponds to the name passed to the [start event](events/start.md)

### Daily

```
    GET /api/analytics/durations/{name}/daily
```


### Response: 200 OK

```json
{
  "name" : "eventname",
  "durations": [
    {
      "year": 2017,
      "month": 2,
      "day": 10,
      "averageDuration": 123
    },
  ]
}
```

### Monthly

```
    GET /api/analytics/durations/{name}/monthly
```


### Response: 200 OK

```json
{
  "name" : "eventname",
  "durations": [
    {
      "year": 2017,
      "month": 2,
      "averageDuration": 123
    },
  ]
}
```



### Yearly

```
    GET /api/analytics/durations/{name}/yearly
```


### Response: 200 OK

```json
{
  "name" : "eventname",
  "durations": [
    {
      "year": 2017,
      "averageDuration": 123
    },
  ]
}
```



## Game durations

Get aggregated game duration statistics by day, month or year


### Daily

```
    GET /api/analytics/game-durations/daily
```


### Response: 200 OK

```json
{
  "durations": [
    {
      "year": 2017,
      "month": 2,
      "day": 10,
      "averageDuration": 123
    },
  ]
}
```

### Monthly

```
    GET /api/analytics/game-durations/monthly
```


### Response: 200 OK

```json
{
  "durations": [
    {
      "year": 2017,
      "month": 2,
      "averageDuration": 123
    },
  ]
}
```



### Yearly

```
    GET /api/analytics/game-durations/yearly
```


### Response: 200 OK

```json
{
  "durations": [
    {
      "year": 2017,
      "averageDuration": 123
    },
  ]
}
```





## Level drop-offs

Get aggregated level dropp-off statistics by day, month or year


### Daily

```
    GET /api/analytics/level-drop-offs/daily
```


### Response: 200 OK

```json
{
  "durations": [
    {
      "year": 2017,
      "month": 2,
      "day": 10,
      "reachedLevel": 2,
      "totalCount": 123
    },
    {
      "year": 2017,
      "month": 2,
      "day": 10,
      "reachedLevel": 3,
      "totalCount": 81
    },
  ]
}
```

### Monthly

```
    GET /api/analytics/level-drop-offs/monthly
```


### Response: 200 OK

```json
{
  "durations": [
    {
      "year": 2017,
      "month": 2,
      "reachedLevel": 2,
      "totalCount": 123
    },
  ]
}
```



### Yearly

```
    GET /api/analytics/level-drop-offs/yearly
```


### Response: 200 OK

```json
{
  "durations": [
    {
      "year": 2017,
      "reachedLevel": 2,
      "totalCount": 123
    },
  ]
}
```



