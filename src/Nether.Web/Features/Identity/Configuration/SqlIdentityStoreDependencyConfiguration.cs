using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Nether.Common.DependencyInjection;
using Nether.Data.EntityFramework.Identity;
using Nether.Data.Identity;
using Nether.Data.Sql.Identity;

namespace Nether.Web.Features.Identity.Configuration
{
    public class SqlIdentityStoreDependencyConfiguration : IdentityStoreDependencyConfigurationBase
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            // configure store and dependencies
            var scopedConfiguration = configuration.GetSection("Identity:Store:properties");
            services.AddSingleton(scopedConfiguration.Get<SqlIdentityContextOptions>());

            services.AddTransient<IdentityContextBase, SqlIdentityContext>();
            services.AddTransient<IUserStore, EntityFrameworkUserStore>();

            // configure type to perform migrations
            services.AddTransient<IDependencyInitializer<IUserStore>, SqlIdentityStoreDependencyConfiguration>();
        }

        public override IApplicationBuilder Use(IApplicationBuilder app)
        {
            var logger = app.ApplicationServices.GetRequiredService<ILogger<SqlIdentityStoreDependencyConfiguration>>();
            logger.LogInformation("Run Migrations for SqlIdentityContext");
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = (SqlIdentityContext)serviceScope.ServiceProvider.GetRequiredService<IdentityContextBase>();
                context.Database.Migrate();
            }

            return base.Use(app);
        }
    }
}
