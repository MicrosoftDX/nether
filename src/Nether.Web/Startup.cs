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
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Nether.Web.Utilities;
using Swashbuckle.AspNetCore.Swagger;
using IdentityServer4;

namespace Nether.Web
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger _logger;

        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Startup>();
            _hostingEnvironment = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            loggerFactory.AddTrace(LogLevel.Information);
            loggerFactory.AddAzureWebAppDiagnostics(); // docs: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging#appservice
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
                //options.OperationFilter<ApiPrefixFilter>();
                options.CustomSchemaIds(type => type.FullName);
                //options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                //{
                //    Type = "oauth2",
                //    Flow = "implicit",
                //    AuthorizationUrl = ""
                //});
            });

            services.AddSingleton<ExceptionLoggingFilterAttribute>();

            // TODO make this conditional with feature switches
            services.AddIdentityServices(Configuration, _logger, _hostingEnvironment);
            services.AddLeaderboardServices(Configuration, _logger);
            services.AddPlayerManagementServices(Configuration, _logger);
            services.AddAnalyticsServices(Configuration, _logger);


            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    PolicyName.NetherIdentityClientId,
                    policy => policy.RequireClaim(
                        "client_id",
                        "nether-identity"
                        ));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<Startup>();


            app.EnsureInitialAdminUser(Configuration, logger);

            // Set up separate web pipelines for identity, MVC UI, and API
            // as they each have different auth requirements!

            app.Map("/identity", idapp =>
            {
                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
                idapp.UseIdentityServer();

                // TODO - add facebook!
            });


            app.Map("/api", apiapp =>
            {
                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

                var idsvrConfig = Configuration.GetSection("Identity:IdentityServer");
                string authority = idsvrConfig["Authority"];
                bool requireHttps = idsvrConfig.GetValue("RequireHttps", true);

                apiapp.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
                {
                    Authority = authority,
                    RequireHttpsMetadata = requireHttps,

                    ApiName = "nether-all",
                    AllowedScopes = { "nether-all" },
                });

                // TODO filter which routes this matches (i.e. only API routes)
                apiapp.UseMvc();

                apiapp.UseSwagger(options =>
                {
                    options.RouteTemplate = "swagger/{documentName}/swagger.json";
                });
                apiapp.UseSwaggerUi(options =>
                {
                    options.RoutePrefix = "swagger/ui";
                    options.SwaggerEndpoint("/api/swagger/v0.1/swagger.json", "v0.1 Docs");
                });
            });

            app.Map("/ui", uiapp =>
            {
                uiapp.UseDefaultFiles();
                uiapp.UseStaticFiles();

                uiapp.UseMvc(); // TODO filter which routes this matches (i.e. only non-API routes)
            });
        }
    }
}

