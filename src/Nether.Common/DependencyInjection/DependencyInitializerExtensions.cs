using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Nether.Common.DependencyInjection
{
    public static class DependencyInitializerExtensions
    {
        public static void Initialize<TDependency>(this IApplicationBuilder app)
        {
            var dependencyExecutor = app.ApplicationServices.GetService<IDependencyInitializer<TDependency>>();
            if (dependencyExecutor != null)
            {
                dependencyExecutor.Use(app);
            }
        }
    }
}
