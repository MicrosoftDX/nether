using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nether.Identity.Development.Configuration;

namespace Nether.Identity.Development
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentityServer(options =>
            {
                options.Endpoints.EnableAuthorizeEndpoint = true;
                options.Endpoints.EnableTokenEndpoint = true;
            })
            .SetTemporarySigningCredential() // using inbuilt signing cert, but we are explicitly a dev-only service ;-)
            .AddInMemoryStores()
            .AddInMemoryClients(Clients.Get())
            .AddInMemoryScopes(Scopes.Get())
            .AddInMemoryUsers(Users.Get())
            ;


            services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Func<string, LogLevel, bool> filter = (scope, level) =>
                scope.StartsWith("IdentityServer") ||
                scope.StartsWith("IdentityModel") ||
                level == LogLevel.Error ||
                level == LogLevel.Critical;

            loggerFactory.AddConsole(filter);
            loggerFactory.AddDebug(filter);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseIdentityServer();

                app.UseStaticFiles();
                app.UseMvcWithDefaultRoute();

            }
            else
            {
                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync("Nether.Identity.Development is only intended for local, development-only use but the environment isn't configured as a development environment");
                });
            }

        }
    }
}
