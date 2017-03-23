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
using Nether.Web.Features.Leaderboard.Configuration;

namespace Nether.Web.Features.Common
{
    public static class ApplicationPerformanceMonitoringExtensions
    {
        private static Dictionary<string, Type> _wellKnownMonitorTypes = new Dictionary<string, Type>
            {
                {"null", typeof(NullMonitorDependencyConfiguration) },
                {"appinsights", typeof(AppInsightsMonitorDependencyConfiguration) },
            };

        public static IServiceCollection AddApplicationPerformanceMonitoring(
            this IServiceCollection services,
            IConfiguration configuration,
            ILogger logger,
            IHostingEnvironment hostingEnvironment)
        {
            services.AddServiceFromConfiguration("Common:ApplicationPerformanceMonitor", _wellKnownMonitorTypes, configuration, logger, hostingEnvironment);

            return services;
        }
    }
}