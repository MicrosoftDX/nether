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

namespace Nether.Web.Features.Identity
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
                .AddTemporarySigningCredential() // using inbuilt signing cert, but we are explicitly a dev-only service at this point ;-)
                .AddInMemoryClients(Clients.Get())
                .AddInMemoryIdentityResources(Scopes.GetIdentityResources())
                .AddInMemoryApiResources(Scopes.GetApiResources())
                .AddExtensionGrantValidator<FacebookUserAccessTokenExtensionGrantValidator>()
            ;
            services.AddTransient<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<IUserStore, InMemoryUserStore>();
            services.AddTransient<IProfileService, StoreBackedProfileService>();
            services.AddTransient<IResourceOwnerPasswordValidator, StoreBackedResourceOwnerPasswordValidator>();

            return services;
        }
    }
}