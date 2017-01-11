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

namespace Nether.Web.Features.Identity
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(
            this IServiceCollection services,
            IConfiguration configuration,
            ILogger logger
,
            IHostingEnvironment hostingEnvironment)
        {
            if (hostingEnvironment.EnvironmentName != "Development")
            {
                throw new NotSupportedException($"The Identity Server configuration is currently only intended for Development environments. Current environment: '{hostingEnvironment.EnvironmentName}'");
            }

            var clientSource = new ConfigurationBasedClientSource(logger);
            var clients = clientSource.LoadClients(configuration.GetSection("Identity:Clients"));

            services.AddIdentityServer(options =>
                {
                    options.Endpoints.EnableAuthorizeEndpoint = true;
                    options.Endpoints.EnableTokenEndpoint = true;
                })
                .AddTemporarySigningCredential() // using inbuilt signing cert, but we are explicitly a dev-only service at this point ;-)
                .AddInMemoryClients(clients)
                .AddInMemoryIdentityResources(Scopes.GetIdentityResources())
                .AddInMemoryApiResources(Scopes.GetApiResources())
                .AddExtensionGrantValidator<FacebookUserAccessTokenExtensionGrantValidator>()
            ;
            services.AddTransient<IPasswordHasher, PasswordHasher>();
            services.AddTransient<IProfileService, StoreBackedProfileService>();
            services.AddTransient<IResourceOwnerPasswordValidator, StoreBackedResourceOwnerPasswordValidator>();

            if (configuration.Exists("Identity:Store:wellKnown"))
            {
                // register using well-known type
                var wellKnownType = configuration["Identity:Store:wellknown"];
                var scopedConfiguration = configuration.GetSection("Identity:Store:properties");
                switch (wellKnownType)
                {
                    case "in-memory":
                        logger.LogInformation("Identity:Store: using 'in-memory' store");
                        services.AddTransient<IUserStore, EntityFrameworkUserStore>();
                        // Add IdentityContextOptions to configure for in-memory
                        services.AddSingleton(new IdentityContextOptions
                        {
                            OnConfiguring = builder =>
                            {
                                builder.UseInMemoryDatabase();
                            }
                        });
                        services.AddSingleton<IdentityContext>(serviceProvider=>
                        {
                            logger.LogInformation("Identity:Store: Adding in-memory seed users...");

                            // construct the singleton so that we can provide seeded users for testing
                            var context = new IdentityContext(
                                    serviceProvider.GetService<ILoggerFactory>(), 
                                    serviceProvider.GetService<IdentityContextOptions>());
                            var seedUsers = InMemoryUsersSeed.Get(serviceProvider.GetService<IPasswordHasher>());
                            context.Users.AddRange(seedUsers.Select(IdentityMappingExtensions.Map));
                            context.SaveChanges();
                            logger.LogInformation("Identity:Store: Adding in-memory seed users... complete");

                            return context;
                        });
                        break;
                    case "sql":
                        logger.LogInformation("Identity:Store: using 'Sql' store");
                        string connectionString = scopedConfiguration["ConnectionString"];
                        services.AddTransient<IUserStore, EntityFrameworkUserStore>();
                        // Add IdentityContextOptions to configure for SQL Server
                        services.AddSingleton(new IdentityContextOptions
                        {
                            OnModelCreating = builder =>
                            {
                                builder.Entity<UserEntity>().ForSqlServerToTable("Users");
                            },
                            OnConfiguring = builder =>
                            {
                                builder.UseSqlServer(connectionString);
                            }
                        });
                        services.AddTransient<IdentityContext>();
                        break;
                    default:
                        throw new Exception($"Unhandled 'wellKnown' type for Identity:Store: '{wellKnownType}'");
                }
            }
            else
            {
                // fall back to generic "factory"/"implementation" configuration
                services.AddServiceFromConfiguration<IUserStore>(configuration, logger, "Identity:Store");
            }


            return services;
        }
        private static T GetServiceFromCollection<T>(IServiceCollection services)
        {
            return (T)services
                .LastOrDefault(d => d.ServiceType == typeof(T))
                ?.ImplementationInstance;
        }

        
    }
}