using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Nether.Common.DependencyInjection;
using Nether.Data.Identity;
using Nether.Data.EntityFramework.Identity;
using Nether.Data.InMemory.Identity;

namespace Nether.Web.Features.Identity.Configuration
{
    public class InMemoryIdentityStoreDependencyConfiguration : IdentityStoreDependencyConfigurationBase
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            // configure store and dependencies
            services.AddTransient<IdentityContextBase, InMemoryIdentityContext>();
            services.AddTransient<IUserStore, EntityFrameworkUserStore>();
        }
    }
}
