// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Nether.Common.DependencyInjection;
using Nether.Data.Identity;
using Nether.Data.EntityFramework.Identity;
using Nether.Data.InMemory.Identity;

namespace Nether.Web.Features.Identity.Configuration
{
    public class InMemoryIdentityStoreDependencyConfiguration : IdentityStoreDependencyConfigurationBase
    {
        protected override void OnConfigureServices(DependencyConfigurationContext context)
        {
            // configure store and dependencies
            context.Services.AddTransient<IdentityContextBase, InMemoryIdentityContext>();
            context.Services.AddTransient<IUserStore, EntityFrameworkUserStore>();

            base.OnConfigureServices(context);
        }
    }
}
