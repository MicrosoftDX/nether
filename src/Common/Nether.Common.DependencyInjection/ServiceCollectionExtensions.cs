// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
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

            if (configuration.Exists($"{serviceName}:implementation:type"))
            {
                string baseConfigKey = $"{serviceName}:implementation";
                var implementationType = GetTypeFromConfiguration(configuration, baseConfigKey);

                services.AddTransient(typeof(TService), implementationType);
            }
            else if (configuration.Exists($"{serviceName}:factory:type"))
            {
                string baseConfigKey = $"{serviceName}:factory";
                var type = GetTypeFromConfiguration(configuration, baseConfigKey);

                var factory = (IDependencyFactory<TService>)Activator.CreateInstance(type);
                Func<IServiceProvider, TService> func = serviceProvider => factory.CreateInstance(serviceProvider);
                services.AddTransient(func);
            }
            else
            {
                throw new ArgumentException("No valid configuration found for '{serviceName}'");
            }
        }


        /// <summary>
        /// Load Type from config using baseConfigKey:type, baseConfigKey:assembly
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="baseConfigKey"></param>
        /// <returns></returns>
        private static Type GetTypeFromConfiguration(IConfiguration configuration, string baseConfigKey)
        {
            string factoryType = configuration[$"{baseConfigKey}:type"];
            string factoryAssembly = configuration[$"{baseConfigKey}:assembly"];
            return Type.GetType($"{factoryType}, {factoryAssembly}");
        }
    }
}

