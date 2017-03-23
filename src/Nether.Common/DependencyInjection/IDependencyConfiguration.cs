using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nether.Common.DependencyInjection
{
    /// <summary>
    /// Base type contract used to provide ways pluggable service configuration
    /// </summary>
    public abstract class DependencyConfiguration
    {
        public IServiceCollection ConfigureServices(string serviceName, IServiceCollection services, IConfiguration configuration, ILogger logger, IHostingEnvironment hostingEnvironment)
        {
            var context = new DependencyConfigurationContext(serviceName, services, configuration, logger, hostingEnvironment);
            OnConfigureServices(context);
            return services;
        }

        protected abstract void OnConfigureServices(DependencyConfigurationContext context);
    }

    public class DependencyConfigurationContext
    {
        public DependencyConfigurationContext(
            string serviceName,
            IServiceCollection services,
            IConfiguration configuration,
            ILogger logger,
            IHostingEnvironment hostingEnvironment
            )
        {
            Services = services;
            Configuration = configuration;
            ScopedConfiguration = configuration.GetSection($"{serviceName}:properties");
            Logger = logger;
            HostingEnvironment = hostingEnvironment;
        }

        public IServiceCollection Services { get; private set; }
        public IConfiguration Configuration { get; private set; }
        public ILogger Logger { get; private set; }
        public IHostingEnvironment HostingEnvironment { get; private set; }
        public IConfigurationSection ScopedConfiguration { get; private set; }
    }
}
