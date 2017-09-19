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
    public class AdminUserApiTests : WebTestBase, IClassFixture<IntegrationTestUsersFixture>
    {
        [Fact]
        public async Task As_a_player_I_get_Forbidden_response_calling_GetUsers()
        {
            var client = await AsPlayerAsync();

            var response = await client.GetAsync("/api/admin/users");
            await response.AssertStatusCodeAsync(HttpStatusCode.Forbidden);
        }
        [Fact]
        public async Task As_a_player_I_get_Forbidden_response_calling_GetUser()
        {
            var client = await AsPlayerAsync();
            var response = await client.GetAsync("/api/admin/users/123");
            await response.AssertStatusCodeAsync(HttpStatusCode.Forbidden);
        }
        // ... should we fill out the other requests to check permissions, or not?


        [Fact]
        public async Task As_an_admin_I_can_list_users()
        {
            var client = await AsAdminAsync();
            var response = await client.GetAsync("/api/admin/users");
            await response.AssertStatusCodeAsync(HttpStatusCode.OK);

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
        public async Task As_an_admin_I_can_create_a_user_and_update_their_details_and_remove_the_user()
        {
            // This test is a classic example of something that would benefit from SpecFlow
            // as it can be split into separate steps that are reported on in the execution output!
            // Roll on SpecFlow support for .NET Core

            var client = await AsAdminAsync();

            // Add user
            var response = await client.PostAsJsonAsync(
                "/api/admin/users",
                new
                {
                    role = "Admin",
                    active = true
                });

            await response.AssertStatusCodeAsync(HttpStatusCode.Created);

            var userLocation = response.Headers.Location.LocalPath;
            Assert.StartsWith("/api/admin/users/", userLocation);


            // Get user
            response = await client.GetAsync(userLocation);

            await response.AssertStatusCodeAsync(HttpStatusCode.OK);
            dynamic userContent = await response.Content.ReadAsAsync<dynamic>();
            dynamic user = userContent.user;

            string userId = user.userId;
            Assert.NotNull(userId);
            Assert.Equal("Admin", (string)user.role);
            Assert.True((bool)user.active);


            // Update user
            response = await client.PutAsJsonAsync(
                userLocation,
                new
                {
                    role = "Player",
                    active = false
                });

            await response.AssertStatusCodeAsync(HttpStatusCode.OK);
            userContent = await response.Content.ReadAsAsync<dynamic>();
            user = userContent.user;
            Assert.Equal(userId, (string)user.userId);
            Assert.Equal("Player", (string)user.role);
            Assert.False((bool)user.active);


            // Get user again
            response = await client.GetAsync(userLocation);

            await response.AssertStatusCodeAsync(HttpStatusCode.OK);
            userContent = await response.Content.ReadAsAsync<dynamic>();
            user = userContent.user;
            Assert.Equal(userId, (string)user.userId);
            Assert.Equal("Player", (string)user.role);
            Assert.False((bool)user.active);


            // Remove user
            response = await client.DeleteAsync(userLocation);
            await response.AssertStatusCodeAsync(HttpStatusCode.NoContent);


            // Get user again (shouldn't exist)
            response = await client.GetAsync(userLocation);
            await response.AssertStatusCodeAsync(HttpStatusCode.NotFound);
        }

        private async Task<HttpClient> AsPlayerAsync()
        {
            return await SignInAsync(username: "testuser");
        }
        private async Task<HttpClient> AsAdminAsync()
        {
            return await SignInAsync(username: "devadmin");
        }
    }
}
