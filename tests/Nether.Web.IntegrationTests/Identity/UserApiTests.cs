// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Nether.Web.IntegrationTests.Identity
{
    public class UserApiTests : WebTestBase
    {
        [Fact]
        public async Task As_a_player_I_get_Forbidden_response_calling_GetUsers()
        {
            var client = await AsPlayerAsync();

            var response = await client.GetAsync("/api/identity/users");
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        [Fact]
        public async Task As_a_player_I_get_Forbidden_response_calling_GetUser()
        {
            var client = await AsPlayerAsync();
            var response = await client.GetAsync("/api/identity/users/123");
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        // ... should we fill out the other requests to check permissions, or not?


        [Fact]
        public async Task As_an_admin_I_can_list_users()
        {
            var client = await AsAdminAsync();
            var response = await client.GetAsync("/api/identity/users");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            dynamic responseContent = await response.Content.ReadAsAsync<dynamic>();
            var users = ((IEnumerable<dynamic>)responseContent.users).ToList();
            Assert.NotNull(users);
            Assert.True(users.Count > 0);

            dynamic user = users[0];
            Assert.NotNull(user.userId);

            Assert.NotNull(user.role);

            Assert.NotNull(user._link);
        }

        [Fact]
        public async Task As_an_admin_I_can_create_a_user_with_a_login_and_login_as_that_user()
        {
            var client = await AsAdminAsync();

            // TODO create user with login
            var response = client.PostAsJsonAsync("/api/identity/users")

            // TODO login as that user
        }


        private async Task<HttpClient> AsPlayerAsync()
        {
            return await GetClientAsync(username: "testuser", setPlayerGamertag: true);
        }
        private async Task<HttpClient> AsAdminAsync()
        {
            return await GetClientAsync(username: "devadmin", setPlayerGamertag: false);
        }

    }
}
