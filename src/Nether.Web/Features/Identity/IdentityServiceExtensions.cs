// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Nether.Common.DependencyInjection;
using Nether.Data.Leaderboard;
using Nether.Data.MongoDB.Leaderboard;
using Nether.Web.Features.Identity.Configuration;
using Microsoft.AspNetCore.Hosting;
using Nether.Web.Features.Identity;

namespace Nether.Web.Features.Leaderboard
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            if (hostingEnvironment.EnvironmentName != "Development")
            {
                throw new NotSupportedException($"The Identity Server configuration is currently only intended for Development environments. Current environment: '{hostingEnvironment.EnvironmentName}'");
            }
            services.AddIdentityServer(options =>
                {
                    options.Endpoints.EnableAuthorizeEndpoint = true;
                    options.Endpoints.EnableTokenEndpoint = true;
                })
                .SetTemporarySigningCredential() // using inbuilt signing cert, but we are explicitly a dev-only service at this point ;-)
                .AddInMemoryStores()
                .AddInMemoryClients(Clients.Get())
                .AddInMemoryScopes(Scopes.Get())
                .AddInMemoryUsers(Users.Get())
                .AddExtensionGrantValidator<FacebookUserAccessTokenExtensionGrantValidator>()
            ;
            return services;
        }
    }
}