// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Nether.Web.IntegrationTests
{
    public class SwaggerTests
    {
        [Fact]
        public async Task GET_Swagger_returns_200_OK()
        {
            var client = new HttpClient();

            var response = await client.GetAsync($"{WebTestBase.BaseUrl}api/swagger/v0.1/swagger.json");

            await response.AssertStatusCodeAsync(HttpStatusCode.OK);
        }
    }
}
