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

        //[Fact]
        //public async Task As_a_player_I_get_Forbidden_response_calling_GetUsers()
        //{
        //    AsPlayer();
        //    ResponseForGet("/users", hasStatusCode: HttpStatusCode.Forbidden);
        //}

        //private void ResponseForGet(string path, HttpStatusCode hasStatusCode)
        //{
        //    _client.GetAsync
        //}

        //private void AsPlayer()
        //{
        //    _client = GetClient(username:"testuser", isPlayer: true);
        //}

    }
}
