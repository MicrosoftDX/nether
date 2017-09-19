// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Nether.Common.DependencyInjection;
using Nether.Common.ApplicationPerformanceMonitoring;

namespace Nether.Web.Features.Leaderboard.Configuration
{
    public class NullMonitorDependencyConfiguration : DependencyConfiguration
    {
        protected override void OnConfigureServices(DependencyConfigurationContext context)
        {
            context.Services.AddSingleton<IApplicationPerformanceMonitor, NullMonitor>();
        }
    }
}
