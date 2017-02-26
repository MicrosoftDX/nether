// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Logging;
using Nether.Web.Features.Analytics.Models.Endpoint;

namespace Nether.Web.Features.Analytics
{
    public static class AnalyticsServiceExtensions
    {
        public static IServiceCollection AddAnalyticsServices(
            this IServiceCollection services,
            IConfiguration configuration,
            ILogger logger)
        {
            services.AddEndpointInfo(configuration, logger, "Analytics:EventHub");

            return services;
        }

        public static IServiceCollection AddEndpointInfo(
            this IServiceCollection services,
            IConfiguration configuration,
            ILogger logger,
            string key)
        {
            var endpointInfo = GetEndpointInfo(configuration.GetSection(key));

            logger.LogInformation("Analytics:EventHub: using resource '{0}'", endpointInfo.Resource);

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