// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Nether.Common.DependencyInjection;
using Nether.Integration.Analytics;
using Nether.Integration.Default.Analytics;

namespace Nether.Web.Features.Analytics
{
    public static class AnalyticsServiceExtensions
    {
        public static IServiceCollection AddAnalyticsServices(this IServiceCollection services, IConfiguration configuration)
        {
            // TODO - look at what can be extracted to generalise this
            if (configuration.Exists("AnalyticsIntegrationClient:wellKnown"))
            {
                // register using well-known type
                var wellKnownType = configuration["AnalyticsIntegrationClient:wellknown"];
                switch (wellKnownType)
                {
                    case "null":
                        services.AddSingleton<IAnalyticsIntegrationClient, AnalyticsIntegrationNullClient>();
                        break;
                    case "default":
                        var scopedConfiguration = configuration.GetSection("AnalyticsIntegrationClient:properties");
                        string baseUrl = scopedConfiguration["AnalyticsBaseUrl"];

                        services.AddTransient<IAnalyticsIntegrationClient>(serviceProvider=>{
                            return new AnalyticsIntegrationClient(baseUrl);
                        });
                        break;
                    default:
                        throw new Exception($"Unhandled 'wellKnown' type for AnalyticsIntegrationClient: '{wellKnownType}'");
                }
            }
            else
            {
                // fall back to generic "factory"/"implementation" configuration
                services.AddServiceFromConfiguration<IAnalyticsIntegrationClient>(configuration, "AnalyticsIntegrationClient");
            }


            services.AddEndpointInfo(configuration, "EventHub");

            return services;
        }

        public static IServiceCollection AddEndpointInfo(this IServiceCollection services, IConfiguration configuration, string key)
        {
            var endpointInfo = GetEndpointInfo(configuration.GetSection(key));
            services.AddSingleton(endpointInfo);
            return services;
        }
        public static EndpointInfo GetEndpointInfo(IConfiguration configuration)
        {
            return new EndpointInfo
            {
                KeyName = configuration["KeyName"],
                AccessKey = configuration["AccessKey"],
                Resource = configuration["Resource"],
                Ttl = TimeSpan.Parse(configuration["Ttl"])
            };
        }
    }
}