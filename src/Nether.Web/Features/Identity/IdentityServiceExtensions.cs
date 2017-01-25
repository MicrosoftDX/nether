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

namespace Nether.Web.Features.Identity
{
    public static class IdentityServiceExtensions
    {
        public static IApplicationBuilder UseIdentityServices(
            this IApplicationBuilder app,
            IConfiguration configuration,
            ILogger logger
            )
        {
            // TODO - this code was copied from Identity Server sample. Need to understand why the map is cleared!
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var idsvrConfig = configuration.GetSection("Identity:IdentityServer");
            string authority = idsvrConfig["Authority"];
            bool requireHttps = idsvrConfig.GetValue("RequireHttps", true);



            // TODO - this code was copied from the Identity Server sample. Once working, revisit this config and see what is needed to wire up with the generic OpenIdConnect helpers
            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
                Authority = authority,
                RequireHttpsMetadata = requireHttps,
                AllowedScopes = { "nether-all" },
                //AutomaticAuthenticate = true // TODO - understand this setting!
            });

            //implicit flow authentication
            /*IdentityServerAuthenticationOptions identityServerValidationOptions = new IdentityServerAuthenticationOptions
            {
                Authority = "http://localhost:5000/",
                AllowedScopes = new List<string> { "nether-all" },
                RequireHttpsMetadata = false,
                ApiSecret = "dataEventRecordsSecret",
                ApiName = "dataEventRecords",
                AutomaticAuthenticate = true,
                SupportedTokens = SupportedTokens.Both,
                // TokenRetriever = _tokenRetriever,
                // required if you want to return a 403 and not a 401 for forbidden responses
                AutomaticChallenge = true,
            };

            app.UseIdentityServerAuthentication(identityServerValidationOptions);*/


            AddInitialAdminUser(app, configuration, logger);

            return app;
        }

        private static void AddInitialAdminUser(IApplicationBuilder app, IConfiguration configuration, ILogger logger)
        {
            try
            {
                var serviceProvider = app.ApplicationServices;

                logger.LogInformation("Identity:Store: Checking user store...");

                // construct a context to test if we have a user
                var identityContext = serviceProvider.GetRequiredService<IdentityContext>();
                bool gotUsers = identityContext.Users.Any();
                if (gotUsers)
                {
                    logger.LogInformation("Identity:Store: users exist - no action");
                }
                else
                {
                    logger.LogInformation("Identity:Store: Adding initial admin user...");
                    // Create an initial admin
                    var passwordHasher = serviceProvider.GetRequiredService<IPasswordHasher>();
                    var password = configuration["Identity:InitialSetup:AdminPassword"];
                    var user = new UserEntity
                    {
                        Role = RoleNames.Admin,
                        IsActive = true,
                        Logins = new List<LoginEntity>
                                    {
                                        new LoginEntity {
                                            ProviderType = LoginProvider.UserNamePassword,
                                            ProviderId = "netheradmin",
                                            ProviderData = passwordHasher.HashPassword(password)
                                        }
                                    }
                    };
                    user.Logins[0].User = user;
                    identityContext.Users.Add(user);
                    identityContext.SaveChanges();
                    logger.LogInformation("Identity:Store: Adding initial admin user... complete");
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical("Identity:Store: Adding initial admin user, exception: {0}", ex);
            }
        }

        public static IServiceCollection AddIdentityServices(
            this IServiceCollection services,
            IConfiguration configuration,
            ILogger logger,
            IHostingEnvironment hostingEnvironment)
        {
            ConfigureIdentityPlayerMangementClient(services, configuration, logger);
            ConfigureIdentityServer(services, configuration, logger, hostingEnvironment);
            ConfigureIdentityStore(services, configuration, logger);

            return services;
        }

        private static void ConfigureIdentityPlayerMangementClient(
            IServiceCollection services,
            IConfiguration configuration,
            ILogger logger)
        {
            if (configuration.Exists("Identity:PlayerManagementClient:wellKnown"))
            {
                // register using well-known type
                var wellKnownType = configuration["Identity:PlayerManagementClient:wellknown"];
                var scopedConfiguration = configuration.GetSection("Identity:PlayerManagementClient:properties");
                switch (wellKnownType)
                {
                    case "default":
                        var baseUri = scopedConfiguration["BaseUrl"];
                        logger.LogInformation("Identity:PlayerManagementClient: using 'default' client with BaseUrl '{0}'", baseUri);

                        // could simplify this by requiring the client secret in the properties for PlayerManagementClient, but that duplicates config
                        var clientSource = new ConfigurationBasedClientSource(logger);
                        var clientSecret = clientSource.GetClientSecret(configuration.GetSection("Identity:Clients"), "nether-identity");
                        if (string.IsNullOrEmpty(clientSecret))
                        {
                            throw new Exception("Unable to determine the client secret for nether-identity");
                        }

                        services.AddSingleton<IIdentityPlayerManagementClient, DefaultIdentityPlayerManagementClient>(serviceProvider =>
                        {
                            return new DefaultIdentityPlayerManagementClient(
                                baseUri,
                                clientSecret,
                                serviceProvider.GetRequiredService<ILoggerFactory>()
                                );
                        });
                        break;
                    default:
                        throw new Exception($"Unhandled 'wellKnown' type for Identity:PlayerManagementClient: '{wellKnownType}'");
                }
            }
            else
            {
                // fall back to generic "factory"/"implementation" configuration
                services.AddServiceFromConfiguration<IUserStore>(configuration, logger, "Identity:PlayerManagementClient");
            }
        }

        private static void ConfigureIdentityServer(
            IServiceCollection services,
            IConfiguration configuration,
            ILogger logger,
            IHostingEnvironment hostingEnvironment)
        {
            if (hostingEnvironment.EnvironmentName != "Development")
            {
                throw new NotSupportedException($"The Identity Server configuration is currently only intended for Development environments. Current environment: '{hostingEnvironment.EnvironmentName}'");
            }

            var clientSource = new ConfigurationBasedClientSource(logger);
            var clients = clientSource.LoadClients(configuration.GetSection("Identity:Clients"))
                                .ToList();

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
            services.AddTransient<UserClaimsProvider>();
        }

        private static void ConfigureIdentityStore(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
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
                        services.AddTransient<IdentityContext>();
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
        }
        
        private static T GetServiceFromCollection<T>(IServiceCollection services)
        {
            return (T)services
                .LastOrDefault(d => d.ServiceType == typeof(T))
                ?.ImplementationInstance;
        }
    }
}