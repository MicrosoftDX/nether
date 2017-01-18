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

            var response = await client.GetAsync("http://localhost:5000/api/swagger/v0.1/swagger.json");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
