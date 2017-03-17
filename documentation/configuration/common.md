# Common configuration

The examples here show the config options for the `appsettings.json` file - see [this documentation](overriding-settings.md) for how to override the settings.

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



## CORS

If you want to be able to access the Nether APIs from a web application hosted from a different domain then you will need to configure [CORS](https://www.w3.org/TR/cors/) for Nether. To do this, set the 

```json
{
    "Common": {
        "Cors": {
            // Add CORS origins to allow here (or via environment variables)
            "AllowedOrigins": []
        }
    }
}
```

## HTTPS Redirection

When running for production purposes, HTTPS should be used as per the [OAuth 2.0 spec](https://tools.ietf.org/html/rfc6749).

To simplify working with Nether, you can configure it to issue HTTP redirects for any plain HTTP requests to HTTPS. This is configured through the setting shown below:

```json
{
    "Common": {
        "RedirectToHttps" : true
    }
}
```


## Application Performance Monitor

Nether has the ability to send telemetry to Application Performance Monitoring solutions. Out of the box we have support for [Application Insights](https://docs.microsoft.com/azure/application-insights/) but there is an abstraction to allow other providers to be [plugged in](dependency-injection.md).


### Configuring Application Insights

This section assumes that you have an instance of Application Insights, if not [create one first](https://docs.microsoft.com/en-us/azure/application-insights/app-insights-create-new-resource).


To configure Application Insights, set the `wellknown` value to `appinsights` as shown below.

Then set `properties:InstrumentationKey` to the instrumentation key for your Application Insights instacen (See how to [get your instrumentation key](https://docs.microsoft.com/en-us/azure/application-insights/app-insights-create-new-resource#copy-the-instrumentation-key))

```json
{
    "Common": {
        "ApplicationPerformanceMonitor": {
            "wellknown": "appinsights",
            "properties" : {
                "InstrumentationKey": "<your key here>"
            }
        }
    }
}
```


### Disabling Application Performance Monitoring

To disable Application Performance Monitoring, set the `wellknown` value to `null`:

```json
{
    "Common": {
        "ApplicationPerformanceMonitor": {
            "wellknown": "null"
        }
    }
}
```