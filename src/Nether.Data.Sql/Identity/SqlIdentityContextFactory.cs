// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nether.Data.Sql.Identity
{
    /// <summary>
    /// Class added to enable creating EF Migrations
    /// See https://docs.microsoft.com/en-us/ef/core/api/microsoft.entityframeworkcore.infrastructure.idbcontextfactory-1
    /// </summary>
    public class SqlIdentityContextFactory : IDbContextFactory<SqlIdentityContext>
    {
        public SqlIdentityContext Create(DbContextFactoryOptions options)
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole();
            var logger = loggerFactory.CreateLogger<SqlIdentityContextFactory>();


            logger.LogInformation("Creating ConfigurationBuilder. ContentRootPath:{0}", options.ContentRootPath);
            // TODO - should find a better way to share the config builder logic!
            var builder = new ConfigurationBuilder()
                                .SetBasePath(options.ContentRootPath)
                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                .AddJsonFile($"appsettings.{options.EnvironmentName}.json", optional: true)
                                .AddEnvironmentVariables();
            var configuration = builder.Build();


            var connectionString = configuration["Identity:Store:properties:ConnectionString"];
            logger.LogInformation("Using connection string: {0}", connectionString);

            return new SqlIdentityContext(
                loggerFactory,
                new SqlIdentityContextOptions
                {
                    ConnectionString = connectionString
                });
        }
    }
}
