// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nether.Data.EntityFramework.Common
{
    public static class ConfigurationHelper
    {
        public static IConfiguration GetConfiguration(ILogger logger, string contentRootPath, string environmentName)
        {
            logger.LogInformation("Creating ConfigurationBuilder. ContentRootPath:{0}", contentRootPath);
            // TODO - should find a better way to share the config builder logic!
            var builder = new ConfigurationBuilder()
                                .SetBasePath(contentRootPath)
                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                                .AddEnvironmentVariables();
            var configuration = builder.Build();
            return configuration;
        }
    }
}
