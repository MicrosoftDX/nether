// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace Nether.Web.IntegrationTests
{
    public static class HttpResponseMessageAssertions
    {
        public static async Task AssertSuccessStatusCodeAsync(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                string body = await GetBodyContentAsync(response);
                throw new XunitException($"NonSuccessStatusCode. Body: {body}");
            }
        }
        public static async Task AssertStatusCodeAsync(this HttpResponseMessage response, HttpStatusCode expectedStatusCode)
        {
            if (response.StatusCode != expectedStatusCode)
            {
                string body = await GetBodyContentAsync(response);
                throw new XunitException($"Expected status code: {expectedStatusCode}, actual {response.StatusCode}. Body: {body}");
            }
        }

        private static async Task<string> GetBodyContentAsync(HttpResponseMessage response)
        {
            string body = null;
            try
            {
                body = await response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            { }

            return body;
        }
    }
}
