// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

using Nether.Common.DependencyInjection;
using Nether.Integration.Identity;
using Nether.Integration.Leaderboard;

namespace Nether.Web.Features.Leaderboard.Configuration
{
    public class DefaultLeaderboardPlayerManagementClientDependencyConfiguration : DependencyConfiguration
    {
        protected override void OnConfigureServices(DependencyConfigurationContext context)
        {
            var identityBaseUri = context.ScopedConfiguration["IdentityBaseUrl"];
            var apiBaseUri = context.ScopedConfiguration["ApiBaseUrl"];
            context.Logger.LogInformation("Leaderboard:PlayerManagementClient: using 'default' client with IdentityBaseUrl '{0}', ApiBaseUrl '{1}'", identityBaseUri, apiBaseUri);

            // could simplify this by requiring the client secret in the properties for PlayerManagementClient, but that duplicates config
            // Should revisit this approach when we try splitting out the services
            var clientSource = new Identity.Configuration.ConfigurationBasedClientSource(context.Logger);
            var clientSecret = clientSource.GetClientSecret(context.Configuration.GetSection("Identity:Clients"), "nether_identity");
            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new Exception("Unable to determine the client secret for nether_identity");
            }

            context.Services.AddSingleton<ILeaderboardPlayerManagementClient, DefaultLeaderboardPlayerManagementClient>(serviceProvider =>
            {
                return new DefaultLeaderboardPlayerManagementClient(
                    identityBaseUri,
                    apiBaseUri,
                    clientSecret,
                    serviceProvider.GetRequiredService<ILogger<DefaultLeaderboardPlayerManagementClient>>()
                    );
            });
        }
    }
}
