using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nether.Leaderboard.Data;
using Nether.Leaderboard.Data.InMemory;
using Nether.Leaderboard.Data.Mongodb;

namespace Nether.Leaderboard.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            //TODO: Fix so that both Swagger and dependency injection is configurable
            services.AddSwaggerGen();

            services.AddTransient<ILeaderboardStore, MongodbLeaderboardStore>(getConfiguration);
        }

        private MongodbLeaderboardStore getConfiguration(IServiceProvider arg)
        {
            string connectionString = Configuration.GetValue<string>("mondbConnectionString");
            return new MongodbLeaderboardStore(connectionString);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
            app.UseSwagger(routeTemplate: "api/swagger/{apiVersion}/swagger.json");
            app.UseSwaggerUi(baseRoute: "api/swagger/ui", swaggerUrl: "/api/swagger/v1/swagger.json");

        }
    }
}
