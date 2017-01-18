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

        private async Task<HttpResponseMessage> ResponseForGetAsync(string path, HttpStatusCode hasStatusCode)
        {
            var response = await _client.GetAsync(path);

            Assert.Equal(hasStatusCode, response.StatusCode);

            return response;
        }

        private async Task AsPlayerAsync()
        {
            _client = await GetClientAsync(username: "testuser", setPlayerGamertag: true);
        }

    }
}
