# Overriding settings

* [Overview of appsettings.json](overview-of-appsettings-json)
* [Overriding configuration with config/appsettings.json](overriding-configuration-with-config-appsettings-json)
* [Overriding configuration with environment variables](overriding-configuration-with-environment-variables)

## Overview of appsettings.json

The default configuration is specified in the appsettings.json under `src/Nether.Web`.


This JSON file contains a top-level property for each nether service (e.g. `leaderboard`, `identity`), as well as some common top-level settings. A snippet of the file is shown below.

```json
{
    "Common": {
        "Cors": {
            "AllowedOrigins": []
        }
    },
    "Leaderboard": {
        "Enabled": true,
        "Store": {
            "wellknown": "in-memory"
        },
        "AnalyticsIntegrationClient": {
            "wellknown": "null"
        },
        "Leaderboards": {
            // config omitted
        }
    },
    "Identity": {
        "Enabled": true,
        "InitialSetup": {
            "AdminPassword": "N3therAdm1n" 
        },
        "PlayerManagementClient": {
            "wellknown": "default",
            "properties": {
                "IdentityBaseUrl": "http://localhost:5000/identity",
                "ApiBaseUrl": "http://localhost:5000/api"
            }
        },
        "Store": {
            "wellknown": "in-memory"
        },
        // config omitted
    }
    // config omitted
}
```


Some aspects of the configuration relate to settings for the services, e.g. the `Identity:InitialSetup:AdminPassword`. Others are used to specify which dependencies should be wired up, e.g. `Leaderboard:Store`.
In the case of dependencies, there is a common pattern that is followed that is discussed further in the [dependency injection docs](dependency-injection.md).

## Overriding configuration with config/appsettings.json

A quick way to modify your settings locally is to specify the values you want to override in the `appsettings.json` file in the `Nether.Web/config` folder. Any values specified in this file will override the defaults in `Nether.Web/appsettings.json`. Additionally, there is a `.gitignore` entry that prevents this file being committed to source control to help avoid accidentally adding secrets to source control when developing Nether.

The config folder also has some sample configuration files, e.g. [appsettings-sql.sample.json](../../src/Nether.Web/config/appsettings-sql.sample.json)


## Overriding configuration with environment variables

An alternative way to override settings is using environment variables.


In the example above for the leaderboard store, the `in-memory` provider was specified using the `wellknown` property:

```json
  "Leaderboard": {
        "Store": {
            "wellknown": "in-memory"
        },
  }
```

To change this to the SQL Server store we can change the configuration `wellknown` value to `sql`. We also need to provide the `properties` object to supply any properties that are required, in this case the connection string to the database:


```json
  "Leaderboard": {
        "Store": {
            "wellknown": "sql",
            "properties": {
              "ConnectionString": "<connection string>"
            }
        },
  }
```

The settings can be changed without editing the config file by setting environment variables. This can be useful for deployment across environments (such as staging, production) as the configuration can be part of the environment rather than the source code. Additionally, if you are contributing to nether then it helps to keep configuration secrets out of source code!


In the example above, we make the same change by setting environment variables called `Leaderboard:Store:wellknown` and `LeaderboardStore:properties:ConnectionString`. Notice that the environment variable name consists of the path down the JSON document separated by colons (`:`). Since colons aren't valid in environment variables on bash, we can also use double underscores (`__`) as separators as shown below.


Powershell:
```powershell
${env:Leaderboard:Store:wellknown} = "sql"
${env:Leaderboard:Store:properties:ConnectionString} = "<put your connection string here>"
``` 

or in bash:
```bash
export Leaderboard__Store__wellknown="sql"
export Leaderboard__Store__properties__ConnectionString="<put your connection string here>"
``` 

When running in [Azure Web Apps](https://docs.microsoft.com/en-us/azure/app-service-web/app-service-web-overview), you can create [Application Settings](https://docs.microsoft.com/en-us/azure/app-service-web/web-sites-configure#application-settings) as these also create environment variables.