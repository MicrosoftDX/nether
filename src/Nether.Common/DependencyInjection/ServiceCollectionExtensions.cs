// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Nether.Common.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServiceFromConfiguration<TService>(
            this IServiceCollection services,
            IConfiguration configuration,
            ILogger logger,
            string serviceName)
        where TService : class
        {
            // TODO - error handling

            if (configuration.Exists($"{serviceName}:implementation:type"))
            {
                logger.LogInformation("{0} - using implementation option...", serviceName);
                string baseConfigKey = $"{serviceName}:implementation";
                var implementationType = GetTypeFromConfiguration(configuration, logger, baseConfigKey);

                services.AddTransient(typeof(TService), implementationType);
            }
            else if (configuration.Exists($"{serviceName}:factory:type"))
            {
                logger.LogInformation("{0} - using factory option...", serviceName);
                string baseConfigKey = $"{serviceName}:factory";
                var type = GetTypeFromConfiguration(configuration, logger, baseConfigKey);

                var factory = (IDependencyFactory<TService>)Activator.CreateInstance(type);
                Func<IServiceProvider, TService> func = serviceProvider => factory.CreateInstance(serviceProvider);
                services.AddTransient(func);
            }
            else
            {
                throw new ArgumentException($"No valid configuration found for '{serviceName}'");
            }
        }


        /// <summary>
        /// Load Type from config using baseConfigKey:type, baseConfigKey:assembly
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="baseConfigKey"></param>
        /// <returns></returns>
        private static Type GetTypeFromConfiguration(IConfiguration configuration, ILogger logger, string baseConfigKey)
        {
            string typeName = configuration[$"{baseConfigKey}:type"];
            string assemblyName = configuration[$"{baseConfigKey}:assembly"];
            logger.LogInformation("... with type '{0} from {1}", typeName, assemblyName);
            var type = Type.GetType($"{typeName}, {assemblyName}");
            if (type == null)
            {
                throw new ArgumentException($"Type not found for '{baseConfigKey}'. Type='{typeName}', Assembly:'{assemblyName}'");
            }
            return type;
        }
    }
}

