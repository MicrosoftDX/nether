// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nether.Common.ApplicationPerformanceMonitoring;
using Nether.Common.DependencyInjection;
using Nether.Data.EntityFramework.Identity;
using Nether.Data.Identity;
using Nether.Data.Sql.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nether.Web.Features.Identity.Configuration
{
    public abstract class IdentityStoreDependencyConfigurationBase : DependencyConfiguration, IDependencyInitializer<IUserStore>
    {
        protected override void OnConfigureServices(DependencyConfigurationContext context)
        {
            // configure type to perform migrations
            context.Services.AddSingleton<IDependencyInitializer<IUserStore>>(this);
        }
        public virtual IApplicationBuilder Use(IApplicationBuilder app)
        {
            var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
            var logger = app.ApplicationServices.GetRequiredService<ILogger<IdentityStoreDependencyConfigurationBase>>();
            EnsureInitialAdminUser(app, configuration, logger);
            return app;
        }

        private void EnsureInitialAdminUser(IApplicationBuilder app, IConfiguration configuration, ILogger logger)
        {
            IApplicationPerformanceMonitor appMonitor = null;
            try
            {
                var serviceProvider = app.ApplicationServices;
                appMonitor = serviceProvider.GetService<IApplicationPerformanceMonitor>();

                logger.LogInformation("Identity:Store: Checking user store...");

                // construct a context to test if we have a user
                var identityContext = serviceProvider.GetRequiredService<IdentityContextBase>();
                bool gotUsers = identityContext.Users.Any(u => u.Role == RoleNames.Admin);
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
                appMonitor.LogError(ex, "Error adding initial admin user");
            }
        }
    }
}
