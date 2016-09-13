using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nether.Leaderboard.Data;
using Nether.Leaderboard.Data.Mongodb;
using System.Reflection.Metadata;
using Nether.Common.DependencyInjection;

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

            services.AddSwaggerGen();

            services.AddTransient<ILeaderboardStore, MongodbLeaderboardStore>(GetConfiguration);
        }

        private MongodbLeaderboardStore GetConfiguration(IServiceProvider arg)
        {
            var connectionString = Configuration.GetValue<string>("MongoDbConnectionString");
            var dbName = Configuration.GetValue<string>("MongoDbDatabaseName");
            return new MongodbLeaderboardStore(connectionString, dbName);
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
