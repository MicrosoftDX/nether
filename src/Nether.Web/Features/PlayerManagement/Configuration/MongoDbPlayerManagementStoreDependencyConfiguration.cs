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
    public class MongoDBPlayerManagementStoreDependencyConfiguration : IDependencyConfiguration
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            var scopedConfiguration = configuration.GetSection("PlayerManagement:Store:properties");
            string databaseName = scopedConfiguration["DatabaseName"];
            string connectionString = scopedConfiguration["ConnectionString"];

            services.AddTransient<IPlayerManagementStore>(serviceProvider =>
            {
                var storeLogger = serviceProvider.GetRequiredService<ILogger<MongoDBPlayerManagementStore>>();
                // TODO - look at encapsulating the connection info and registering that so that we can just register the type without the factory
                return new MongoDBPlayerManagementStore(connectionString, databaseName, storeLogger);
            });
        }
    }
}
