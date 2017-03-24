// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

using Nether.Common.DependencyInjection;
using Nether.Data.Analytics;
using Nether.Data.EntityFramework.Analytics;
using Nether.Data.InMemory.Analytics;
using Nether.Data.Sql.Analytics;
using Nether.Web.Features.Analytics.Configuration;
using Nether.Web.Features.Analytics.Models.Endpoint;
using Nether.Web.Utilities;
using Microsoft.AspNetCore.Hosting;

namespace Nether.Web.Features.Analytics
{
    public static class AnalyticsServiceExtensions
    {
        private static Dictionary<string, Type> s_wellKnownStoreTypes = new Dictionary<string, Type>
            {
                {"in-memory", typeof(InMemoryAnalyticsStoreDependencyConfiguration) },
                {"sql", typeof(SqlAnalyticsStoreDependencyConfiguration) },
            };


        public static IServiceCollection AddAnalyticsServices(
            this IServiceCollection services,
            IConfiguration configuration,
            ILogger logger,
            NetherServiceSwitchSettings serviceSwitches,
            IHostingEnvironment hostingEnvironment
            )
        {
            bool enabled = configuration.GetValue<bool>("Analytics:Enabled");
            if (!enabled)
            {
                logger.LogInformation("Analytics service not enabled");
                return services;
            }
            logger.LogInformation("Configuring Analytics service");
            serviceSwitches.AddServiceSwitch("Analytics", true);


            services.AddEndpointInfo(configuration, logger, "Analytics:EventHub");
            services.AddServiceFromConfiguration("Analytics:Store", s_wellKnownStoreTypes, configuration, logger, hostingEnvironment);


            return services;
        }

        private static IServiceCollection AddEndpointInfo(
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