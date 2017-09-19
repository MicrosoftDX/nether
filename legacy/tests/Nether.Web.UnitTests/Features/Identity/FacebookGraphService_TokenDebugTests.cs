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
using Nether.Web.Features.Identity;
using Newtonsoft.Json.Linq;

namespace Nether.Web.UnitTests.Features.Identity
{
    public class FacebookGraphService_TokenDebugTests
    {
        [Fact]
        public void ParsesTopLevelError()
        {
            var json = @"
{
    'error': {
        'message': 'test message',
        'type' : 'test type',
        'code' : 123
    }
}";
            var result = ParseTokenFromJson(json);

            Assert.NotNull(result.Error);
            Assert.Equal("test message", result.Error.Message);
            Assert.Equal("test type", result.Error.Type);
            Assert.Equal(123, result.Error.Code);

            Assert.False(result.IsValid);
        }

        [Fact]
        public void ParsesDataError()
        {
            var json = @"
{
    'data': {
        'error': {
            'message': 'test message',
            'code' : 123
        },
        'is_valid': false,
        'scopes': []
    }
}";
            var result = ParseTokenFromJson(json);

            Assert.NotNull(result.Error);
            Assert.Equal("test message", result.Error.Message);
            Assert.Equal(123, result.Error.Code);

            Assert.False(result.IsValid);
        }


        [Fact]
        public void ParsesData()
        {
            var json = @"
{
    'data': {
        'is_valid': true,
        'scopes': [ 'scope1', 'scope2'],
        'user_id': '123456',
        'expires_at': 1501167600
    }
}";
            var result = ParseTokenFromJson(json);

            Assert.Null(result.Error);

            Assert.True(result.IsValid);
            Assert.Equal("123456", result.UserId);
            Assert.Equal(new DateTime(2017, 7, 27, 15, 0, 0, DateTimeKind.Utc), result.ExpiresAt);
            Assert.Equal(2, result.Scopes.Length);
            Assert.Equal("scope1", result.Scopes[0]);
            Assert.Equal("scope2", result.Scopes[1]);
        }




        [Fact]
        public void ParsesDataAndError()
        {
            var json = @"
{
    'data': {
        'error': {
            'message': 'test message',
            'code' : 123
        },
        'is_valid': false,
        'scopes': [ 'scope1', 'scope2'],
        'user_id': '123456',
        'expires_at': 1501167600
    }
}";
            var result = ParseTokenFromJson(json);

            Assert.NotNull(result.Error);
            Assert.Equal("test message", result.Error.Message);
            Assert.Equal(123, result.Error.Code);

            Assert.False(result.IsValid);
            Assert.Equal("123456", result.UserId);
            Assert.Equal(new DateTime(2017, 7, 27, 15, 0, 0, DateTimeKind.Utc), result.ExpiresAt);
            Assert.Equal(2, result.Scopes.Length);
            Assert.Equal("scope1", result.Scopes[0]);
            Assert.Equal("scope2", result.Scopes[1]);
        }



        private FacebookTokenDebugResult ParseTokenFromJson(string json)
        {
            dynamic body = JObject.Parse(json);
            var result = FacebookGraphService.ParseTokenDebugResult(body);
            return result;
        }
    }
}
