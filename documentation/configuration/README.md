# Configuration

The configuration for Nether.Web is handled by `appsettings.json` and environment variables.

See the following articles for information on configuring different services in Nether

* [Identity services](identity.md)
* [Leaderboard](leaderboard.md)
* [Player Management](player-management.md)
* TODO Analytics


There is also documentation on how to [override configuration settings using environment variables](appsettings-vs-env-vars.md) rather than by modifying `appsettings.json`.

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