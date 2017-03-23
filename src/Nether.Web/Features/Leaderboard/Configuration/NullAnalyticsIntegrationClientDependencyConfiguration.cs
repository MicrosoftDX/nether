// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether.Common.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nether.Integration.Analytics;
using Nether.Integration.Default.Analytics;

namespace Nether.Web.Features.Leaderboard.Configuration
{
    public class NullAnalyticsIntegrationClientDependencyConfiguration : DependencyConfiguration
    {
        protected override void OnConfigureServices(DependencyConfigurationContext context)
        {
            context.Services.AddSingleton<IAnalyticsIntegrationClient, AnalyticsIntegrationNullClient>();
        }
    }
}
