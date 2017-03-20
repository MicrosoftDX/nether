// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

using IdentityServer4.Services;
using IdentityServer4.Validation;

using Nether.Data.Identity;
using Nether.Web.Features.Identity.Configuration;
using Nether.Common.DependencyInjection;
using Nether.Data.Sql.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using IdentityServer4.Models;
using System.Collections.Generic;
using Nether.Integration.Identity;
using Microsoft.AspNetCore.Builder;
using System.IdentityModel.Tokens.Jwt;
using Nether.Common.ApplicationPerformanceMonitoring;

namespace Nether.Web.Features.Common
{
    public static class ApplicationPerformanceMonitoringExtensions
    {
        public static IServiceCollection AddApplicationPerformanceMonitoring(
            this IServiceCollection services,
            IConfiguration configuration,
            ILogger logger,
            IHostingEnvironment hostingEnvironment)
        {
            ConfigureApplicationPerformanceMonitor(services, configuration, logger, hostingEnvironment);

            return services;
        }

        private static void ConfigureApplicationPerformanceMonitor(
            IServiceCollection services,
            IConfiguration configuration,
            ILogger logger,
            IHostingEnvironment hostingEnvironment)
        {
            if (configuration.Exists("Common:ApplicationPerformanceMonitor:wellKnown"))
            {
                // register using well-known type
                var wellKnownType = configuration["Common:ApplicationPerformanceMonitor:wellknown"];
                var scopedConfiguration = configuration.GetSection("Common:ApplicationPerformanceMonitor:properties");
                switch (wellKnownType)
                {
                    case "null":
                        logger.LogInformation("Common:ApplicationPerformanceMonitor: using 'null' client");
                        services.AddSingleton<IApplicationPerformanceMonitor, NullMonitor>();
                        break;

                    case "appinsights":
                        var instrumentationKey = scopedConfiguration["InstrumentationKey"];
                        logger.LogInformation("Common:ApplicationPerformanceMonitor: using 'appinsights' client with InstrumentationKey '{0}'", instrumentationKey);

                        services.AddApplicationInsightsTelemetry(options =>
                        {
                            options.DeveloperMode = hostingEnvironment.IsDevelopment();
                            options.InstrumentationKey = instrumentationKey;
                        });

                        services.AddTransient<IApplicationPerformanceMonitor, ApplicationInsightsMonitor>();
                        break;

                    default:
                        throw new Exception($"Unhandled 'wellKnown' type for Common:ApplicationPerformanceMonitor: '{wellKnownType}'");
                }
            }
            else
            {
                // fall back to generic "factory"/"implementation" configuration
                services.AddServiceFromConfiguration<IUserStore>(configuration, logger, "Common:ApplicationPerformanceMonitor");
            }
        }
    }
}