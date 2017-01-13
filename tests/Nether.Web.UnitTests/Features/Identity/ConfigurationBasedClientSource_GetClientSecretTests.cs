// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;
using Nether.Web.Features.Identity.Configuration;
using Nether.Web.UnitTests.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Nether.Web.UnitTests.Features.Identity
{
    public class ConfigurationBasedClientSource_GetClientSecretTests : JsonConfigTestBase
    {
        [Fact]
        public void WhenNoMatchingClientId_ReturnsNull()
        {
            var json = @"
{
    'Identity': {
        'Clients': [
            {
                'Id': 'an-id'
            },
            {
                'Id': 'client-id'
            }
        ]
    },
}";
            var clientSecret = GetClientSecretFromJson(json, "random-id");

            Assert.Null(clientSecret);
        }

        [Fact]
        public void WhenMatchingClientHasNoSecret_ReturnsNull()
        {
            var json = @"
{
    'Identity': {
        'Clients': [
            {
                'Id': 'an-id',
                'ClientSecrets': [ 'wibble' ]
            },
            {
                'Id': 'client-id'
            }
        ]
    },
}";
            var clientSecret = GetClientSecretFromJson(json, "client-id");

            Assert.Null(clientSecret);
        }


        [Fact]
        public void WhenMatchingClientHasSecret_ReturnsSecret()
        {
            var json = @"
{
    'Identity': {
        'Clients': [
            {
                'Id': 'an-id'
            },
            {
                'Id': 'client-id',
                'ClientSecrets': [ 'wibble' ]
            }
        ]
    },
}";
            var clientSecret = GetClientSecretFromJson(json, "client-id");

            Assert.Equal("wibble", clientSecret);
        }
        private string GetClientSecretFromJson(string json, string clientId)
        {
            var config = LoadConfig(json);

            var source = new ConfigurationBasedClientSource(NullLogger.Instance);
            var clientSecret = source.GetClientSecret(config.GetSection("Identity:Clients"), clientId);
            return clientSecret;
        }
    }
}
