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
    public class DefaultIdentityPlayerManagementClientDependencyConfiguration : IDependencyConfiguration
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            var scopedConfiguration = configuration.GetSection("Identity:PlayerManagementClient:properties");
            var identityBaseUri = scopedConfiguration["IdentityBaseUrl"];
            var apiBaseUri = scopedConfiguration["ApiBaseUrl"];
            logger.LogInformation("Identity:PlayerManagementClient: using 'default' client with IdentityBaseUrl '{0}', ApiBaseUrl '{1}'", identityBaseUri, apiBaseUri);

            // could simplify this by requiring the client secret in the properties for PlayerManagementClient, but that duplicates config
            var clientSource = new ConfigurationBasedClientSource(logger);
            var clientSecret = clientSource.GetClientSecret(configuration.GetSection("Identity:Clients"), "nether_identity");
            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new Exception("Unable to determine the client secret for nether_identity");
            }

            services.AddSingleton<IIdentityPlayerManagementClient, DefaultIdentityPlayerManagementClient>(serviceProvider =>
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
