# Score Event

[TODO] Discuss if the following fields are relevant/required at all:
- geohashCenterLat
- geoHashCenterLon
- rnd

## When?

## Why?

## Request

### JSON Body
```json
{
    "id": "0_11",
    "type": "geo-location",
    "version": "1.0.0",
    "enqueueTimeUtc": "2017-05-16 T19:43:33.2420000Z",
    "gameSessionId": "A3A22EE1-563A-4697-9EDF-B69B998CD214",
    "lat": "",
    "lon": "",
    "geoHash": "",
    "geoHashPrecision": "",
    "geoHashCenterLat": "",
    "geoHashCenterLon": "",
    "rnd": ""
}
```

Element name       | Required | Type   | Description
------------------ | -------- | ------ | -----------
id              | Yes      | String | TODO
type              | Yes      | String | Specifies the type of event being sent. Has to be "geo-location".
version            | Yes      | String | Specifies the version of event, based on how much information is being sent.
enqueueTimeUtc      | Yes      | DateTime | Specifies the UTC timestamp of the message being enqueued.
gameSessionId      | Yes      | String | GUID that uniquely identifies the game session to correlate with the corresponding game-start/heartbeat event.
lat      | Yes      | float | Latitude of location
lon      | Yes      | float | Longitude of location
geoHash      | Yes      | double | Geohash in which the location is
geoHashPrecision      | Yes      | double | Geohash in which the location is
geoHashCenterLat      | Yes      | float | Latitude of the centre point within given geohash
geoHashCenterLon      | Yes      | float | Longitude of the centre point within given geohash
rnd | Yes | String | random number