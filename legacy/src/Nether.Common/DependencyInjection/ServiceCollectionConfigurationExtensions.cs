// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Nether.Common.DependencyInjection
{
    public static class ServiceCollectionConfigurationExtensions
    {
        public static void AddServiceFromConfiguration(
          this IServiceCollection services,
          string serviceName,
          IDictionary<string, Type> wellKnownTypes,
          IConfiguration configuration,
          ILogger logger,
          IHostingEnvironment hostingEnvironment)
        {
            Type configurationType;
            // TODO - look at what can be extracted to generalise this
            if (configuration.Exists($"{serviceName}:wellKnown"))
            {
                // register using well-known type
                var wellKnownType = configuration[$"{serviceName}:wellknown"];
                if (!wellKnownTypes.TryGetValue(wellKnownType, out configurationType))
                {
                    throw new Exception($"Unhandled 'wellKnown' type for {serviceName}: '{wellKnownType}'");
                }
                logger.LogInformation($"{serviceName}: using '{wellKnownType}' well known type");
            }
            else if (configuration.Exists($"{serviceName}:configureWith"))
            {
                // fall back to generic "configureWith" configuration
                configurationType = configuration.LoadTypeFromConfiguration($"{serviceName}:configureWith", logger);
                logger.LogInformation($"{serviceName}: using '{configurationType.FullName}' configuration type");
            }
            else
            {
                throw new Exception($"No configuration specified for {serviceName}");
            }

            DependencyConfiguration dependencyConfiguration;
            try
            {
                dependencyConfiguration = (DependencyConfiguration)Activator.CreateInstance(configurationType);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unhandled exception loading configuration type for {serviceName}", ex);
            }

            dependencyConfiguration.ConfigureServices(serviceName, services, configuration, logger, hostingEnvironment);
        }


        /// <summary>
        /// Load Type from config using baseConfigKey:type, baseConfigKey:assembly
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="baseConfigKey"></param>
        /// <returns></returns>
        public static Type LoadTypeFromConfiguration(this IConfiguration configuration, string baseConfigKey, ILogger logger)
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

