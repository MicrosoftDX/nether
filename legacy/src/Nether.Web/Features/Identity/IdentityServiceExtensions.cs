// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Collections.Generic;

using IdentityServer4.Services;
using IdentityServer4.Validation;

using Nether.Common.DependencyInjection;
using Nether.Web.Features.Identity.Configuration;
using Nether.Web.Utilities;

namespace Nether.Web.Features.Identity
{
    public static class IdentityServiceExtensions
    {
        private static Dictionary<string, Type> s_wellKnownStoreTypes = new Dictionary<string, Type>
            {
                {"in-memory", typeof(InMemoryIdentityStoreDependencyConfiguration) },
                {"sql", typeof(SqlIdentityStoreDependencyConfiguration) },
            };
        private static Dictionary<string, Type> s_wellKnownPlayerManagementClientTypes = new Dictionary<string, Type>
            {
                {"default", typeof(DefaultIdentityPlayerManagementClientDependencyConfiguration) },
            };

        public static IServiceCollection AddIdentityServices(
            this IServiceCollection services,
            IConfiguration configuration,
            ILogger logger,
            NetherServiceSwitchSettings serviceSwitches,
            IHostingEnvironment hostingEnvironment)
        {
            bool enabled = configuration.GetValue<bool>("Identity:Enabled");
            if (!enabled)
            {
                logger.LogInformation("Identity service not enabled");
                return services;
            }
            logger.LogInformation("Configuring Identity service");
            serviceSwitches.AddServiceSwitch("Identity", true);
            serviceSwitches.AddServiceSwitch("IdentityUi", true);

            services.AddServiceFromConfiguration("Identity:PlayerManagementClient", s_wellKnownPlayerManagementClientTypes, configuration, logger, hostingEnvironment);
            ConfigureIdentityServer(services, configuration, logger, hostingEnvironment);
            services.AddServiceFromConfiguration("Identity:Store", s_wellKnownStoreTypes, configuration, logger, hostingEnvironment);

            return services;
        }
        private static void ConfigureIdentityServer(
            IServiceCollection services,
            IConfiguration configuration,
            ILogger logger,
            IHostingEnvironment hostingEnvironment)
        {
            if (!hostingEnvironment.IsDevelopment())
            {
                throw new NotSupportedException($"The Identity Server configuration is currently only intended for Development environments. Current environment: '{hostingEnvironment.EnvironmentName}'");
            }

            var clientSource = new ConfigurationBasedClientSource(logger);
            var clients = clientSource.LoadClients(configuration.GetSection("Identity:Clients"))
                                .ToList();

            var identityServerBuilder = services.AddIdentityServer(options =>
                {
                    options.Endpoints.EnableAuthorizeEndpoint = true;
                    options.Endpoints.EnableTokenEndpoint = true;
                    options.UserInteraction.ErrorUrl = "/account/error";
                })
                .AddTemporarySigningCredential() // using inbuilt signing cert, but we are explicitly a dev-only service at this point ;-)
                .AddInMemoryClients(clients)
                .AddInMemoryIdentityResources(Scopes.GetIdentityResources())
                .AddInMemoryApiResources(Scopes.GetApiResources())
            ;

            // Facebook Sign-in method

            //var facebookUserAccessTokenEnabled = bool.Parse(configuration["Identity:SignInMethods:Facebook:EnableAccessToken"] ?? "false");
            //if (facebookUserAccessTokenEnabled)
            //{
            //    identityServerBuilder.AddExtensionGrantValidator<FacebookUserAccessTokenExtensionGrantValidator>();
            //}

            identityServerBuilder.AddGrantValidatorIfConfigured<FacebookUserAccessTokenExtensionGrantValidator>("Identity:SignInMethods:Facebook:EnableAccessToken", configuration);
            identityServerBuilder.AddGrantValidatorIfConfigured<GuestAccessTokenExtensionGrantValidator>("Identity:SignInMethods:GuestAccess:Enabled", configuration);


            // Guest access token sign-in 
            services.AddTransient<IPasswordHasher, PasswordHasher>();
            services.AddTransient<IProfileService, StoreBackedProfileService>();
            services.AddTransient<IResourceOwnerPasswordValidator, StoreBackedResourceOwnerPasswordValidator>();
            services.AddTransient<UserClaimsProvider>();
            services.AddTransient<FacebookGraphService>();
        }

        private static void AddGrantValidatorIfConfigured<TValidator>(this IIdentityServerBuilder builder, string configurationOption, IConfiguration configuration)
            where TValidator : class, IExtensionGrantValidator
        {
            var option = false;
            bool.TryParse(configuration[configurationOption] ?? bool.FalseString, out option);

            builder.AddGrantValidatorWithCondition<TValidator>(() => option);
        }

        private static void AddGrantValidatorWithCondition<TValidator>(this IIdentityServerBuilder builder, Func<bool> condition)
        where TValidator : class, IExtensionGrantValidator
        {
            if (!condition())
            {
                return;
            }

            builder.AddExtensionGrantValidator<TValidator>();
        }
    }
}