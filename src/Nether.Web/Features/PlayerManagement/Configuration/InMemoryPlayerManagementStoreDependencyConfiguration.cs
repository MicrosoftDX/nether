using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Nether.Common.DependencyInjection;
using Nether.Data.PlayerManagement;
using Nether.Data.EntityFramework.PlayerManagement;
using Nether.Data.InMemory.PlayerManagement;

namespace Nether.Web.Features.PlayerManagement.Configuration
{
    public class InMemoryPlayerManagementStoreDependencyConfiguration : DependencyConfiguration
    {
        protected override void OnConfigureServices(DependencyConfigurationContext context)
        {
            // configure store and dependencies
            context.Services.AddTransient<PlayerManagementContextBase, InMemoryPlayerManagementContext>();
            context.Services.AddTransient<IPlayerManagementStore, EntityFrameworkPlayerManagementStore>();
        }
    }
}
