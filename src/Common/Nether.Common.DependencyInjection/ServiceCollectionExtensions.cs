using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Nether.Common.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServiceFromConfiguration<TService>(
            this IServiceCollection services, 
            IConfiguration configuration, 
            string serviceName)
        where TService : class
        {
            // TODO - error handling
            // TODO - test for implementation, then factory
            string factoryType = configuration[$"{serviceName}:factory:type"];
            string factoryAssembly = configuration[$"{serviceName}:factory:assembly"];
            var type = Type.GetType($"{factoryType}, {factoryAssembly}");

            var factory = (IDependencyFactory<TService>)Activator.CreateInstance(type);
            Func<IServiceProvider, TService> func = serviceProvider =>
            {
                // TODO - should we create custom config that is scoped to the properties section?
                return factory.CreateInstance(configuration); 
            };
            services.AddTransient(func);
        }
    }
}
