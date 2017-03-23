using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Nether.Common.DependencyInjection;
using Nether.Data.EntityFramework.Identity;
using Nether.Data.Identity;
using Nether.Data.Sql.Identity;
using Nether.Integration.Identity;
using System;

namespace Nether.Web.Features.Identity.Configuration
{
    public class DefaultIdentityPlayerManagementClientDependencyConfiguration : DependencyConfiguration
    {
        protected override void OnConfigureServices(DependencyConfigurationContext context)
        {
            var identityBaseUri = context.ScopedConfiguration["IdentityBaseUrl"];
            var apiBaseUri = context.ScopedConfiguration["ApiBaseUrl"];
            context.Logger.LogInformation("Identity:PlayerManagementClient: using 'default' client with IdentityBaseUrl '{0}', ApiBaseUrl '{1}'", identityBaseUri, apiBaseUri);

            // could simplify this by requiring the client secret in the properties for PlayerManagementClient, but that duplicates config
            var clientSource = new ConfigurationBasedClientSource(context.Logger);
            var clientSecret = clientSource.GetClientSecret(context.Configuration.GetSection("Identity:Clients"), "nether_identity");
            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new Exception("Unable to determine the client secret for nether_identity");
            }

            context.Services.AddSingleton<IIdentityPlayerManagementClient, DefaultIdentityPlayerManagementClient>(serviceProvider =>
            {
                return new DefaultIdentityPlayerManagementClient(
                    identityBaseUri,
                    apiBaseUri,
                    clientSecret,
                    serviceProvider.GetRequiredService<ILogger<DefaultIdentityPlayerManagementClient>>()
                    );
            });
        }
    }
}
