// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nether.Web.Features.Analytics;
using Nether.Web.Features.Identity;
using Nether.Web.Features.Leaderboard;
using Nether.Web.Features.PlayerManagement;
using Swashbuckle.Swagger.Model;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using Microsoft.Extensions.PlatformAbstractions;

namespace Nether.Web
{
    public class Startup
    {
        private IHostingEnvironment HostingEnvironment { get; set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            HostingEnvironment = env;
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);


            // Add framework services.
            services.AddMvc();

            services.AddSwaggerGen();
            services.ConfigureSwaggerGen(options =>
            {
                string commentsPath = Path.Combine(
                    PlatformServices.Default.Application.ApplicationBasePath,
                    "Nether.Web.xml");

                options.IncludeXmlComments(commentsPath);
            });

            //services.ConfigureSwaggerGen(options =>
            //{
            //    options.SingleApiVersion(new Info
            //    {
            //        Version = "1.0.0-beta",
            //        Title = "Project Nether",
            //        Description = "Building blocks for gaming on Microsoft Azure",
            //        License = new License { Name = "MIT", Url = "https://github.com/dx-ted-emea/nether/blob/master/LICENSE" }
            //    });
            //});

            // TODO make this conditional with feature switches

            services.AddIdentityServices(Configuration, HostingEnvironment);
            services.AddLeaderboardServices(Configuration);
            services.AddPlayerManagementServices(Configuration);
            services.AddAnalyticsServices(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // TODO - this code was copied from Identity Server sample. Need to understand why the map is cleared!
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            // TODO - this code was copied from the Identity Server sample. Once working, revisit this config and see what is needed to wire up with the generic OpenIdConnect helpers
            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
                Authority = "http://localhost:5000",
                RequireHttpsMetadata = false, // we're dev-only ;-)
                AllowedScopes = { "nether-all" },
                //AutomaticAuthenticate = true // TODO - understand this setting!
            });

            app.UseIdentityServer();
            app.UseMvc();
            app.UseSwagger(routeTemplate: "api/swagger/{apiVersion}/swagger.json");
            app.UseSwaggerUi(baseRoute: "api/swagger/ui", swaggerUrl: "/api/swagger/v1/swagger.json");
        }
    }
}

