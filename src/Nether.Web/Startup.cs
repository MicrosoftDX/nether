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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Nether.Web.Utilities;

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
            services
                .AddMvc(options =>
                {
                    options.Filters.AddService(typeof(ExceptionLoggingFilterAttribute));
                })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter
                    {
                        CamelCaseText = true
                    });
                });

            services.ConfigureSwaggerGen(options =>
            {
                string commentsPath = Path.Combine(
                    PlatformServices.Default.Application.ApplicationBasePath,
                    "Nether.Web.xml");

                options.IncludeXmlComments(commentsPath);
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v0.1", new Info
                {
                    Version = "v0.1",
                    Title = "Project Nether",
                    License = new License
                    {
                        Name = "MIT",
                        Url = "https://github.com/dx-ted-emea/nether/blob/master/LICENSE"
                    }
                });
                //options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                //{
                //    Type = "oauth2",
                //    Flow = "implicit",
                //    AuthorizationUrl = ""
                //});
            });

            services.AddSingleton<ExceptionLoggingFilterAttribute>();

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

            #region [ Admin Web UI ]
            // Create a custom route for Admin Web UI
            // todo: make it nicer
            const string adminFeatureSubstringUrl = "/features/adminwebui";
            app.Use(async (context, next) =>
            {
                await next();

                string path = context.Request.Path.Value;
                if(path.Contains(adminFeatureSubstringUrl))
                {
                    context.Request.Path = adminFeatureSubstringUrl + "/index.html";

                    await next();
                }

            });
            app.UseDefaultFiles();
            app.UseStaticFiles();
            #endregion

            app.UseIdentityServer();
            app.UseMvc();

            app.UseSwagger(routeTemplate: "api/swagger/{documentName}/swagger.json");
            app.UseSwaggerUi(options =>
            {
                options.BaseRoute = "api/swagger/ui";
                options.SwaggerEndpoint("/api/swagger/v0.1/swagger.json", "v0.1 Docs");
            });
        }
    }
}

