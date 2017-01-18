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
    public class ConfigurationBasedClientSource_LoadClientsTests : JsonConfigTestBase
    {
        [Fact]
        public void ClientIdIsParsed()
        {
            var json = @"
{
    'Identity': {
        'Clients': {
            'client-id': {
                'Name' : 'wibble'
            }
        }
    },
}";
            var clients = GetClientsFromJson(json);

            Assert.Equal(1, clients.Count);

            var client = clients[0];
            Assert.Equal("client-id", client.ClientId);
        }


        [Fact]
        public void ClientNameIsParsed()
        {
            var json = @"
{
    'Identity': {
        'Clients': {
            'client-id': {
                'Name': 'client-name'
            }
        }
    },
}";
            var clients = GetClientsFromJson(json);

            Assert.Equal(1, clients.Count);

            var client = clients[0];
            Assert.Equal("client-name", client.ClientName);
        }



        [Fact]
        public void AllowedGrantTypesAreParsed()
        {
            var json = @"
{
    'Identity': {
        'Clients': {
            'client-id': {
                'AllowedGrantTypes': ['grant1', 'grant2']
            }
        }
    },
}";
            var clients = GetClientsFromJson(json);

            Assert.Equal(1, clients.Count);

            var client = clients[0];
            Assert.Equal(
                new[] { "grant1", "grant2" },
                client.AllowedGrantTypes);
        }


        [Fact]
        public void RedirectUrisAreParsed()
        {
            var json = @"
{
    'Identity': {
        'Clients': {
            'client-id': {
                'RedirectUris': ['http://localhost:12345', 'http://example.com']
            }
        }
    },
}";
            var clients = GetClientsFromJson(json);

            Assert.Equal(1, clients.Count);

            var client = clients[0];
            Assert.Equal(
                new[] { "http://localhost:12345", "http://example.com" },
                client.RedirectUris);
        }

        [Fact]
        public void PostLogoutRedirectUrisAreParsed()
        {
            var json = @"
{
    'Identity': {
        'Clients': {
            'client-id': {
                'PostLogoutRedirectUris': ['http://localhost:12345/loggedout', 'http://example.com/done']
            }
        }
    },
}";
            var clients = GetClientsFromJson(json);

            Assert.Equal(1, clients.Count);

            var client = clients[0];
            Assert.Equal(
                new[] { "http://localhost:12345/loggedout", "http://example.com/done" },
                client.PostLogoutRedirectUris);
        }

        [Fact]
        public void AccessTokenTypeIsParsed()
        {
            var json = @"
{
    'Identity': {
        'Clients': {
            'client-id': {
                'AccessTokenType': 'Reference'
            }
        }
    },
}";
            var clients = GetClientsFromJson(json);

            Assert.Equal(1, clients.Count);

            var client = clients[0];
            Assert.Equal(AccessTokenType.Reference, client.AccessTokenType);
        }

        [Fact]
        public void AllowAccessTokensViaBrowserIsParsed()
        {
            var json = @"
{
    'Identity': {
        'Clients': {
            'client-id': {
                'AllowAccessTokensViaBrowser': true
            }
        }
    },
}";
            var clients = GetClientsFromJson(json);

            Assert.Equal(1, clients.Count);

            var client = clients[0];
            Assert.Equal(true, client.AllowAccessTokensViaBrowser);
        }

        [Fact]
        public void AllowedCorsOriginsAreParsed()
        {
            var json = @"
{
    'Identity': {
        'Clients': {
            'client-id': {
                'AllowedCorsOrigins': ['http://localhost:12345/', 'http://example.com/']
            }
        }
    },
}";
            var clients = GetClientsFromJson(json);

            Assert.Equal(1, clients.Count);

            var client = clients[0];
            Assert.Equal(
                new[] { "http://localhost:12345/", "http://example.com/" },
                client.AllowedCorsOrigins);
        }

        [Fact]
        public void UnknownPropertyIsIgnored()
        {
            // could add a test that it is logged


            var json = @"
{
    'Identity': {
        'Clients': {
            'client-id': {
                'Id': 'client-id',
                'SomeRandomProperty': 'wibble'
            }
        }
    },
}";
            var clients = GetClientsFromJson(json);

            Assert.Equal(1, clients.Count);

            var client = clients[0];
            Assert.Equal("client-id", client.ClientId);
        }


        [Fact]
        public void ClientSecretsAreParsedAndHashed()
        {
            const string testHash = "n4bQgYhMfWWaL+qgxVrQFaO/TxsrC4Is0V1sFbDwCgg=";
            const string test2Hash = "YDA64iuZiGG847KPM+7BvnWKITyGyTwHbb6fVYwRx1I=";

            var json = @"
{
    'Identity': {
        'Clients': {
            'client-id': {
                'ClientSecrets': ['test', 'test2']
            }
        }
    },
}";
            var clients = GetClientsFromJson(json);

            Assert.Equal(1, clients.Count);

            var client = clients[0];
            Assert.Equal(
                new[] { testHash, test2Hash },
                client.ClientSecrets.Select(s => s.Value));
        }

        [Fact]
        public void AllowedScopesAreParsed()
        {
            var json = @"
{
    'Identity': {
        'Clients': {
            'client-id': {
                'AllowedScopes': ['scope1', 'scope2']
            }
        }
    },
}";
            var clients = GetClientsFromJson(json);

            Assert.Equal(1, clients.Count);

            var client = clients[0];
            Assert.Equal(
                new[] { "scope1", "scope2" },
                client.AllowedScopes);
        }


        [Fact]
        public void MultiplePropertiesAndClientsAreParsed()
        {
            const string testHash = "n4bQgYhMfWWaL+qgxVrQFaO/TxsrC4Is0V1sFbDwCgg=";

            var json = @"
{
    'Identity': {
        'Clients': {
            'client1': {
                'Name': 'Client 1',
                'AllowedGrantTypes': [ 'hybrid', 'password', 'fb-usertoken' ],
                'RedirectUris': [ 'http://localhost:5000/signin-oidc' ],
                'PostLogoutRedirectUris': [ 'http://localhost:5000/' ],
                'ClientSecrets': [ 'test' ], 
                'AllowedScopes': [ 'openid', 'profile' ]
            },
            'client2': {
                'Name': 'Client 2',
            },
        }
    },
}";
            var clients = GetClientsFromJson(json);

            Assert.Equal(2, clients.Count);

            var client1 = clients[0];
            Assert.Equal("client1", client1.ClientId);
            Assert.Equal("Client 1", client1.ClientName);
            Assert.Equal(
                new[] { "hybrid", "password", "fb-usertoken" },
                client1.AllowedGrantTypes);
            Assert.Equal(
                new[] { "http://localhost:5000/signin-oidc" },
                client1.RedirectUris);
            Assert.Equal(
                new[] { "http://localhost:5000/" },
                client1.PostLogoutRedirectUris);
            Assert.Equal(
                new[] { testHash },
                client1.ClientSecrets.Select(s => s.Value));
            Assert.Equal(
                new[] { "openid", "profile" },
                client1.AllowedScopes);


            var client2 = clients[1];
            Assert.Equal("client2", client2.ClientId);
            Assert.Equal("Client 2", client2.ClientName);
        }


        [Fact]
        public void AllowedGrantTypesAreParsedFromEnvironmentVariable()
        {
            var json = @"
{
    'Identity': {
        'Clients': {
            'client-id': {
                'AllowedGrantTypes': ['grant1', 'grant2']
            }
        }
    },
}";
            Environment.SetEnvironmentVariable("UNITTEST_Identity:Clients:client-id2:AllowedGrantTypes", "mygrant1, mygrant2");
            var clients = GetClientsFromJson(
                json,
                addEnvironmentVariables: true,
                environmentVariablePrefix: "UNITTEST_");

            Assert.Equal(2, clients.Count);

            var client = clients.FirstOrDefault(c => c.ClientId == "client-id2");
            Assert.NotNull(client);
            Assert.Equal(
                new[] { "mygrant1", "mygrant2" },
                client.AllowedGrantTypes);
        }

        private List<Client> GetClientsFromJson(
            string json,
            bool addEnvironmentVariables = false,
            string environmentVariablePrefix = null)
        {
            var config = LoadConfig(json, addEnvironmentVariables, environmentVariablePrefix);

            var source = new ConfigurationBasedClientSource(NullLogger.Instance);
            var clients = source.LoadClients(config.GetSection("Identity:Clients"))
                .ToList();
            return clients;
        }
    }
}
