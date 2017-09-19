// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Nether.Common.DependencyInjection
{
    public static class ServiceCollectionInitializationExtensions
    {
        public static void AddTransientWithOneTimeInitialization<TService>(
            this IServiceCollection services,
            ILogger logger,
            Func<IServiceProvider, TService> factory,
            Action<IServiceProvider> initialization)
        where TService : class
        {
            bool initialised = false;
            object synclock = new object();
            services.AddTransient(serviceProvider =>
            {
                // one-off initialisation
                if (!initialised)
                {
                    lock (synclock)
                    {
                        if (!initialised)
                        {
                            logger.LogInformation($"Performing one-time initialisation for {typeof(TService).FullName}...");

                            initialization(serviceProvider);

                            initialised = true;
                        }
                    }
                }
                return factory(serviceProvider);
            });
        }
    }
}

