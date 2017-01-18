// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Nether.Web.UnitTests.Utilities
{
    public abstract class JsonConfigTestBase : IDisposable
    {
        private string _filename;
        protected IConfiguration LoadConfig(
            string json,
            bool addEnvironmentVariables = false,
            string environmentVariablePrefix = null)
        {
            _filename = Path.GetTempFileName();

            File.WriteAllText(_filename, json);

            var builder = new ConfigurationBuilder()
                .AddJsonFile(_filename);

            if (addEnvironmentVariables)
                builder.AddEnvironmentVariables(environmentVariablePrefix);

            return builder.Build();
        }
        public void Dispose()
        {
            // clean up temporary config file
            if (File.Exists(_filename))
                File.Delete(_filename);
        }
    }
}
