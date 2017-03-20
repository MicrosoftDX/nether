using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Nether.Common.DependencyInjection;
using Nether.Data.EntityFramework.PlayerManagement;
using Nether.Data.Sql.PlayerManagement;
using Nether.Data.PlayerManagement;

namespace Nether.Web.Features.PlayerManagement.Configuration
{
    public class SqlPlayerManagementStoreDependencyConfiguration : IDependencyConfiguration, IDependencyInitializer<IPlayerManagementStore>
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            // configure store and dependencies
            var scopedConfiguration = configuration.GetSection("PlayerManagement:Store:properties");
            services.AddSingleton(scopedConfiguration.Get<SqlPlayerManagementContextOptions>());

            services.AddTransient<PlayerManagementContextBase, SqlPlayerManagementContext>();
            services.AddTransient<IPlayerManagementStore, EntityFrameworkPlayerManagementStore>();

            // configure type to perform migrations
            services.AddTransient<IDependencyInitializer<IPlayerManagementStore>, SqlPlayerManagementStoreDependencyConfiguration>();
        }

        public IApplicationBuilder Use(IApplicationBuilder app)
        {
            var logger = app.ApplicationServices.GetRequiredService<ILogger<SqlPlayerManagementStoreDependencyConfiguration>>();
            logger.LogInformation("Run Migrations for SqlPlayerManagementContext");
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = (SqlPlayerManagementContext)serviceScope.ServiceProvider.GetRequiredService<PlayerManagementContextBase>();
                context.Database.Migrate();
            }

            return app;
        }
    }
}
