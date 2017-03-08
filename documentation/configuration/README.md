# Configuration

To understand how you can specify configuration settings, see [overriding settings](overriding-settings.md).


To find out what settings you can specify see the following sections:

* [Identity services](identity.md)
* [Leaderboard](leaderboard.md)
* [Player Management](player-management.md)
* TODO Analytics
* TODO Common



## Disabling nether services

Each service in Nether is designed to be self-contained, so while they are all packaged in Nether.Web the aim is that you can enable and disable each individual service. In the config, each service has an `Enabled` property that determines whether that service will be run.

E.g.

```json
 {
     "Leaderboard": {
        "Enabled": true,
        "Store": {
            "wellknown": "in-memory"
        },
        // config omitted
    }
 }
```