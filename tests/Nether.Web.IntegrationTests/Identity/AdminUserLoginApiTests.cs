// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Nether.Web.IntegrationTests.Identity
{
    public class AdminUserLoginApiTests : WebTestBase, IClassFixture<IntegrationTestUsersFixture>
    {
        [Fact()]
        public async Task As_an_admin_I_can_create_a_user_with_a_login_and_login_as_that_user()
        {
            var client = await AsAdminAsync();

            // create user
            var response = await client.PostAsJsonAsync(
                "/api/admin/users",
                new
                {
                    role = "Player",
                    active = true
                });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var userLocation = response.Headers.Location.LocalPath;

            // create login
            string username = Guid.NewGuid().ToString();
            const string password = "TestPassword";
            response = await client.PutAsJsonAsync(
                $"{userLocation}/logins/password",
                new
                {
                    username,
                    password
                });

            await response.AssertStatusCodeAsync(HttpStatusCode.Created);

            var loginLocation = response.Headers.Location.LocalPath;
            Assert.NotNull(loginLocation);
            Assert.Equal(userLocation + "/logins/password", loginLocation);

            // Get the user again and verify the login is present in the returned data
            response = await client.GetAsync(userLocation);

            await response.AssertStatusCodeAsync(HttpStatusCode.OK);
            var userContent = await response.Content.ReadAsAsync<dynamic>();
            var user = userContent.user;
            var logins = ((JArray)user.logins).Cast<dynamic>().ToList();
            Assert.Equal(1, logins.Count);

            var login = logins[0];
            Assert.Equal("password", (string)login.providerType);
            Assert.Equal(username, (string)login.providerId);
            Assert.Equal($"/api/admin/users/{user.userId}/logins/password", (string)login._link);


            // login as that user
            var newClient = await AsPlayerAsync(username, password);

            // delete the user
            response = await client.DeleteAsync(userLocation);
            await response.AssertStatusCodeAsync(HttpStatusCode.NoContent);
        }


        private async Task<HttpClient> AsPlayerAsync(string username = "testuser", string password = null)
        {
            return await SignInAsync(username, password);
        }
        private async Task<HttpClient> AsAdminAsync()
        {
            return await SignInAsync("devadmin");
        }
    }
}
