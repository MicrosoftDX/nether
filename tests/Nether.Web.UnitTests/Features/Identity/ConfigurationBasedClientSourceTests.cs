// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;
using Nether.Web.Features.Identity.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Nether.Web.UnitTests.Features.Identity
{
    public class ConfigurationBasedClientSourceTests : IDisposable
    {
        [Fact]
        public void ClientIdIsParsed()
        {
            var json = @"
{
    'Identity': {
        'Clients': [
            {
                'Id': 'client-id'
            }
        ]
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
        'Clients': [
            {
                'Name': 'client-name'
            }
        ]
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
        'Clients': [
            {
                'AllowedGrantTypes': ['grant1', 'grant2']
            }
        ]
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
        'Clients': [
            {
                'RedirectUris': ['http://localhost:12345', 'http://example.com']
            }
        ]
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
        'Clients': [
            {
                'PostLogoutRedirectUris': ['http://localhost:12345/loggedout', 'http://example.com/done']
            }
        ]
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
        'Clients': [
            {
                'AccessTokenType': 'Reference'
            }
        ]
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
        'Clients': [
            {
                'AllowAccessTokensViaBrowser': true
            }
        ]
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
        'Clients': [
            {
                'AllowedCorsOrigins': ['http://localhost:12345/', 'http://example.com/']
            }
        ]
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
        'Clients': [
            {
                'Id': 'client-id',
                'SomeRandomProperty': 'wibble'
            }
        ]
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
        'Clients': [
            {
                'ClientSecrets': ['test', 'test2']
            }
        ]
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
        'Clients': [
            {
                'AllowedScopes': ['scope1', 'scope2']
            }
        ]
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
        'Clients': [
            {
                'Id': 'client1',
                'Name': 'Client 1',
                'AllowedGrantTypes': [ 'hybrid', 'password', 'fb-usertoken' ],
                'RedirectUris': [ 'http://localhost:5000/signin-oidc' ],
                'PostLogoutRedirectUris': [ 'http://localhost:5000/' ],
                'ClientSecrets': [ 'test' ], 
                'AllowedScopes': [ 'openid', 'profile' ]
            },
            {
                'Id': 'client2',
                'Name': 'Client 2',
            },
        ]
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


        //        var json = @"
        //{
        //    'Identity': {
        //        'Clients': [
        //          {
        //            'Id': 'devclient',
        //            'Name': 'Dev Client',
        //            'AllowedGrantTypes': [ 'hybrid', 'password', 'fb-usertoken' ],
        //            'RedirectUris': [ 'http://localhost:5000/signin-oidc' ],
        //            'PostLogoutRedirectUris': [ 'http://localhost:5000/' ],
        //            'ClientSecrets': [ 'devsecret' ], // TODO: should this be plain, or Sha-hashed?
        //            'AllowedScopes': [ 'openid', 'profile', 'nether-all' ]
        //        }
        //        ]
        //    },
        //}";

        private List<IdentityServer4.Models.Client> GetClientsFromJson(string json)
        {
            var config = LoadConfig(json);

            var source = new ConfigurationBasedClientSource(NullLogger.Instance);
            var clients = source.LoadClients(config.GetSection("Identity:Clients"))
                .ToList();
            return clients;
        }

        private string _filename;
        private IConfiguration LoadConfig(string json)
        {
            // TODO - need to clean this up!
            _filename = Path.GetTempFileName();

            File.WriteAllText(_filename, json);

            return new ConfigurationBuilder()
                .AddJsonFile(_filename)
                .Build();
        }
        public void Dispose()
        {
            // clean up temporary config file
            if (File.Exists(_filename))
                File.Delete(_filename);
        }
    }
}
