// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Nether.Common.DependencyInjection;
using Nether.Data.Analytics;
using Nether.Data.EntityFramework.Analytics;
using Nether.Data.InMemory.Analytics;

namespace Nether.Web.Features.Analytics.Configuration
{
    public class InMemoryAnalyticsStoreDependencyConfiguration : DependencyConfiguration
    {
        protected override void OnConfigureServices(DependencyConfigurationContext context)
        {
            // configure store and dependencies
            context.Services.AddTransient<AnalyticsContextBase, InMemoryAnalyticsContext>();
            context.Services.AddTransient<IAnalyticsStore, EntityFrameworkAnalyticsStore>();
        }
    }
}
