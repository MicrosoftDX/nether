// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Nether.Common.DependencyInjection;
using Nether.Data.PlayerManagement;
using Nether.Data.EntityFramework.PlayerManagement;
using Nether.Data.InMemory.PlayerManagement;
using Nether.Data.MongoDB.PlayerManagement;

namespace Nether.Web.Features.PlayerManagement.Configuration
{
    public class MongoDBPlayerManagementStoreDependencyConfiguration : DependencyConfiguration
    {
        protected override void OnConfigureServices(DependencyConfigurationContext context)
        {
            string databaseName = context.ScopedConfiguration["DatabaseName"];
            string connectionString = context.ScopedConfiguration["ConnectionString"];

            context.Services.AddTransient<IPlayerManagementStore>(serviceProvider =>
            {
                var storeLogger = serviceProvider.GetRequiredService<ILogger<MongoDBPlayerManagementStore>>();
                // TODO - look at encapsulating the connection info and registering that so that we can just register the type without the factory
                return new MongoDBPlayerManagementStore(connectionString, databaseName, storeLogger);
            });
        }
    }
}
