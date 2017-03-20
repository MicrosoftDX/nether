using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Nether.Common.DependencyInjection;
using Nether.Data.EntityFramework.Analytics;
using Nether.Data.Sql.Analytics;
using Nether.Data.Analytics;

namespace Nether.Web.Features.Analytics.Configuration
{
    public class SqlAnalyticsStoreDependencyConfiguration : IDependencyConfiguration, IDependencyInitializer<IAnalyticsStore>
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            // configure store and dependencies
            var scopedConfiguration = configuration.GetSection("PlayerManagement:Store:properties");
            services.AddSingleton(scopedConfiguration.Get<SqlAnalyticsContextOptions>());

            services.AddTransient<AnalyticsContextBase, SqlAnalyticsContext>();
            services.AddTransient<IAnalyticsStore, EntityFrameworkAnalyticsStore>();

            // configure type to perform migrations
            services.AddTransient<IDependencyInitializer<IAnalyticsStore>, SqlAnalyticsStoreDependencyConfiguration>();
        }

        public IApplicationBuilder Use(IApplicationBuilder app)
        {
            var logger = app.ApplicationServices.GetRequiredService<ILogger<SqlAnalyticsStoreDependencyConfiguration>>();
            logger.LogInformation("Run Migrations for SqlPlayerManagementContext");
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = (SqlAnalyticsContext)serviceScope.ServiceProvider.GetRequiredService<AnalyticsContextBase>();
                context.Database.Migrate();
            }

            return app;
        }
    }
}
