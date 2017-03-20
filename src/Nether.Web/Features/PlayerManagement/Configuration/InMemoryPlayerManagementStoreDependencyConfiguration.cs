using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Nether.Common.DependencyInjection;
using Nether.Data.PlayerManagement;
using Nether.Data.EntityFramework.PlayerManagement;
using Nether.Data.InMemory.PlayerManagement;

namespace Nether.Web.Features.PlayerManagement.Configuration
{
    public class InMemoryPlayerManagementStoreDependencyConfiguration : IDependencyConfiguration
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            // configure store and dependencies
            services.AddTransient<PlayerManagementContextBase, InMemoryPlayerManagementContext>();
            services.AddTransient<IPlayerManagementStore, EntityFrameworkPlayerManagementStore>();
        }
    }
}
