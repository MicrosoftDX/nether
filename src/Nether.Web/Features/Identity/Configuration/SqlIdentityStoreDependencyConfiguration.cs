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
        protected override void OnConfigureServices(DependencyConfigurationContext context)
        {
            // configure store and dependencies
            context.Services.AddSingleton(context.ScopedConfiguration.Get<SqlIdentityContextOptions>());

            context.Services.AddTransient<IdentityContextBase, SqlIdentityContext>();
            context.Services.AddTransient<IUserStore, EntityFrameworkUserStore>();

            // configure type to perform migrations
            context.Services.AddTransient<IDependencyInitializer<IUserStore>, SqlIdentityStoreDependencyConfiguration>();
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
