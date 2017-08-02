// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.PlatformAbstractions;

using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;

using IdentityServer4;
using IdentityServer4.Models;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using Swashbuckle.AspNetCore.Swagger;

using Nether.Common.DependencyInjection;
using Nether.Common.ApplicationPerformanceMonitoring;
using Nether.Data.Leaderboard;
using Nether.Data.PlayerManagement;
using Nether.Data.Identity;
using Nether.Data.Analytics;
using Nether.Web.Features.Analytics;
using Nether.Web.Features.Common;
using Nether.Web.Features.Identity;
using Nether.Web.Features.Leaderboard;
using Nether.Web.Features.PlayerManagement;
using Nether.Web.Utilities;
using Microsoft.Extensions.Options;

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
                .AddJsonFile($"config/appsettings.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            loggerFactory.AddTrace(LogLevel.Information);
            loggerFactory.AddAzureWebAppDiagnostics(); // docs: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging#appservice

            _logger.LogInformation(@"
 _   _      _   _
| \ | | ___| |_| |__   ___ _ __
|  \| |/ _ \ __| '_ \ / _ \ '__|
| |\  |  __/ |_| | | |  __/ |
|_| \_|\___|\__|_| |_|\___|_|
 -        Nether.Web        -
");
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddApplicationPerformanceMonitoring(Configuration, _logger, _hostingEnvironment);

            // Initialize switches for nether services
            var serviceSwitches = new NetherServiceSwitchSettings();
            services.AddSingleton(serviceSwitches);

            services.AddOptions();
            services.Configure<SignInMethodOptions>(Configuration.GetSection("Identity:SignInMethods"));

            // Add framework services.
            services
                .AddMvc(options =>
                {
                    options.Conventions.Add(new FeatureConvention());
                    options.Filters.AddService(typeof(ExceptionLoggingFilterAttribute));
                    options.InputFormatters.Add(new PlainTextInputFormatter());
                })
                .ConfigureApplicationPartManager(partManager =>
                {
                    // swap out the default ControllerFeatureProvider for ours which filters based on the nether service switches
                    var defaultProvider = partManager.FeatureProviders.FirstOrDefault(p => p is ControllerFeatureProvider);
                    if (defaultProvider != null)
                    {
                        partManager.FeatureProviders.Remove(defaultProvider);
                    }
                    partManager.FeatureProviders.Add(new NetherServiceControllerFeatureProvider(serviceSwitches));
                })
                .AddRazorOptions(options =>
                {
                    // {0} - Action Name
                    // {1} - Controller Name
                    // {2} - Area Name
                    // {3} - Feature Name
                    options.AreaViewLocationFormats.Clear();
                    options.AreaViewLocationFormats.Add("/Areas/{2}/Features/{3}/Views/{1}/{0}.cshtml");
                    options.AreaViewLocationFormats.Add("/Areas/{2}/Features/{3}/Views/{0}.cshtml");
                    options.AreaViewLocationFormats.Add("/Areas/{2}/Features/Views/Shared/{0}.cshtml");
                    options.AreaViewLocationFormats.Add("/Areas/Shared/{0}.cshtml");

                    // replace normal view location entirely
                    options.ViewLocationFormats.Clear();
                    options.ViewLocationFormats.Add("/Features/{3}/Views/{1}/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Features/{3}/Views/Shared/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Features/{3}/Views/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Features/Views/Shared/{0}.cshtml");

                    options.ViewLocationExpanders.Add(new FeatureViewLocationExpander());
                })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter
                    {
                        CamelCaseText = true
                    });
                });

            services.AddCors();

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
                        Url = "https://github.com/MicrosoftDX/nether/blob/master/LICENSE"
                    }
                });
                options.CustomSchemaIds(type => type.FullName);
                options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "implicit",
                    AuthorizationUrl = "/identity/connect/authorize",
                    Scopes = new Dictionary<string, string>
                    {
                        { "nether-all", "nether API access" },
                    }
                });

                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            services.AddSingleton<ExceptionLoggingFilterAttribute>();


            services.AddIdentityServices(Configuration, _logger, serviceSwitches, _hostingEnvironment);
            services.AddLeaderboardServices(Configuration, _logger, serviceSwitches, _hostingEnvironment);
            services.AddPlayerManagementServices(Configuration, _logger, serviceSwitches, _hostingEnvironment);
            services.AddAnalyticsServices(Configuration, _logger, serviceSwitches, _hostingEnvironment);


            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    PolicyName.NetherIdentityClientId,
                    policy => policy.RequireClaim(
                        "client_id",
                        "nether_identity"
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

            bool redirectToHttps = bool.Parse(Configuration["Common:RedirectToHttps"] ?? "false");
            if (redirectToHttps)
            {
                _logger.LogInformation("Enabling RedirectToHttps");
                var rewriteOptions = new RewriteOptions()
                                            .AddRedirectToHttps();
                app.UseRewriter(rewriteOptions);
            }

            var serviceSwitchSettings = app.ApplicationServices.GetRequiredService<NetherServiceSwitchSettings>();

            app.Initialize<IUserStore>();
            app.Initialize<IPlayerManagementStore>();
            app.Initialize<ILeaderboardStore>();
            app.Initialize<IAnalyticsStore>();

            app.Initialize<IApplicationPerformanceMonitor>();


            // Set up separate web pipelines for identity, MVC UI, and API
            // as they each have different auth requirements!
            if (serviceSwitchSettings.IsServiceEnabled("Identity"))
            {
                var options = app.ApplicationServices.GetRequiredService<IOptions<SignInMethodOptions>>().Value;

                _logger.LogInformation($"Guest auth enabled: {options.GuestAccess.Enabled}");
                _logger.LogInformation($"Facebook implicit flow enabled: {options.Facebook.EnableImplicit}");
                _logger.LogInformation($"Facebook token flow enabled: {options.Facebook.EnableAccessToken}");
                _logger.LogInformation($"UsernamePassword sign-up enabled: {options.UsernamePassword.AllowUserSignUp}");


                app.Map("/identity", idapp =>
                {
                    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
                    idapp.UseIdentityServer();

                    idapp.UseCookieAuthentication(new CookieAuthenticationOptions
                    {
                        AuthenticationScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,

                        AutomaticAuthenticate = false,
                        AutomaticChallenge = false
                    });

                    var facebookEnabled = options.Facebook.EnableImplicit;
                    if (facebookEnabled)
                    {
                        var appId = options.Facebook.AppId;
                        var appSecret = options.Facebook.AppSecret;

                        idapp.UseFacebookAuthentication(new Microsoft.AspNetCore.Builder.FacebookOptions()
                        {
                            DisplayName = "Facebook",
                            SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,

                            CallbackPath = "/signin-facebook",

                            AppId = appId,
                            AppSecret = appSecret
                        });
                    }

                    idapp.UseStaticFiles();
                    idapp.UseMvc(routes =>
                    {
                        routes.MapRoute(
                            name: "account",
                            template: "account/{action}",
                            defaults: new { controller = "Account" });
                    });
                });
            }

            app.Map("/api", apiapp =>
            {
                apiapp.UseCors(options =>
                {
                    logger.LogInformation("CORS options:");
                    var config = Configuration.GetSection("Common:Cors");

                    var allowedOrigins = config.ParseStringArray("AllowedOrigins").ToArray();
                    logger.LogInformation("AllowedOrigins: {0}", string.Join(",", allowedOrigins));
                    options.WithOrigins(allowedOrigins);

                    // TODO - allow configuration of headers/methods
                    options
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });

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
                apiapp.UseSwaggerUI(options =>
                {
                    options.RoutePrefix = "swagger/ui";
                    options.SwaggerEndpoint("/api/swagger/v0.1/swagger.json", "v0.1 Docs");
                    options.ConfigureOAuth2("swaggerui", "swaggeruisecret".Sha256(), "swagger-ui-realm", "Swagger UI");
                });
            });

            app.Map("/ui", uiapp =>
            {
                uiapp.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    AuthenticationScheme = "Cookies"
                });

                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

                var authority = Configuration["Identity:IdentityServer:Authority"];
                var uiBaseUrl = Configuration["Identity:IdentityServer:UiBaseUrl"];

                // hybrid
                uiapp.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
                {
                    AuthenticationScheme = "oidc",
                    SignInScheme = "Cookies",

                    Authority = authority,
                    RequireHttpsMetadata = false,

                    PostLogoutRedirectUri = uiBaseUrl,

                    ClientId = "mvc2",
                    ClientSecret = "secret",

                    ResponseType = "code id_token",
                    Scope = { "api1", "offline_access" },

                    GetClaimsFromUserInfoEndpoint = true,
                    SaveTokens = true
                });

                uiapp.UseMvc(); // TODO filter which routes this matches (i.e. only non-API routes)
            });

            // serve Landing page static files at root
            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = "",
                FileProvider = new PhysicalFileProvider(Path.Combine(_hostingEnvironment.WebRootPath, "Features", "LandingPage"))
            });
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "landing-page",
                    template: "",
                    defaults: new { controller = "LandingPage", action = "Index" }
                    );
            });
        }
    }
}

