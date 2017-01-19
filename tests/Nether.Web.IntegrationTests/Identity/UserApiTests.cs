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
        private HttpClient _client;

        [Fact]
        public async Task As_a_player_I_get_Forbidden_response_calling_GetUsers()
        {
            await AsPlayerAsync();
            await ResponseForGetAsync("/api/identity/users", hasStatusCode: HttpStatusCode.Forbidden);
        }
        [Fact]
        public async Task As_a_player_I_get_Forbidden_response_calling_GetUser()
        {
            await AsPlayerAsync();
            await ResponseForGetAsync("/api/identity/users/123", hasStatusCode: HttpStatusCode.Forbidden);
        }
        // ... should we fill out the other requests to check permissions, or not?


        [Fact]
        public async Task As_an_admin_I_can_list_users()
        {
            await AsAdminAsync();
            var response = await ResponseForGetAsync("/api/identity/users", hasStatusCode: HttpStatusCode.OK);
            dynamic responseContent = await response.Content.ReadAsAsync<dynamic>();

            var users = ((IEnumerable<dynamic>)responseContent.users).ToList();
            Assert.NotNull(users);
            Assert.True(users.Count > 0);

            dynamic user = users[0];
            Assert.NotNull(user.userId);

            Assert.NotNull(user.role);

            Assert.NotNull(user._link);
        }


        private async Task AsPlayerAsync()
        {
            _client = await GetClientAsync(username: "testuser", setPlayerGamertag: true);
        }
        private async Task AsAdminAsync()
        {
            _client = await GetClientAsync(username: "devadmin", setPlayerGamertag: false);
        }


        private async Task<HttpResponseMessage> ResponseForGetAsync(string path, HttpStatusCode hasStatusCode)
        {
            var response = await _client.GetAsync(path);

            Assert.Equal(hasStatusCode, response.StatusCode);

            return response;
        }
    }
}
