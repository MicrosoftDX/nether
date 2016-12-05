using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace Nether.Web.IntegrationTests
{
    /// <summary>
    /// Base class for integration test giving you an authenticated client ready to work with
    /// </summary>
    public class WebTestBase
    {
        private const string BaseUrl = "http://localhost:5000/";
        private const string ClientId = "resourceowner-test";
        private const string ClientSecret = "devsecret";
        protected const string GamerTag = "testusertagWebTestBase";

        private static readonly Dictionary<string, string> UserToPassword =
            new Dictionary<string, string>
            {
                { "testuser", "testuser" },
                { "devadmin", "devadmin" }
            };

        private static HttpClient CreateClient(string baseUrl)
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = new System.Net.CookieContainer()
            };

            return new HttpClient(handler) { BaseAddress = new Uri(baseUrl) };
        }

        protected HttpClient GetClient(string username)
        {
            HttpClient client = CreateClient(BaseUrl);
            string password = UserToPassword[username];

            //authenticate so it's ready for use
            DiscoveryResponse disco = DiscoveryClient.GetAsync(BaseUrl).Result;
            if (disco.TokenEndpoint == null)
            {
                throw new AuthenticationException("could not discover endpoint, server is offline?");
            }

            var tokenClient = new TokenClient(disco.TokenEndpoint, ClientId, ClientSecret);


            TokenResponse tokenResponse = tokenClient.RequestResourceOwnerPasswordAsync(username, password, "nether-all").Result;
            if (tokenResponse.IsError)
            {
                throw new AuthenticationException("filed to authenticate");
            }
            client.SetBearerToken(tokenResponse.AccessToken);

            // todo: remove this gamertag dark magic
            var token = new JwtSecurityToken(tokenResponse.AccessToken);
            var player = new
            {
                gamertag = GamerTag,
                country = "UK",
                customTag = nameof(WebTestBase)
            };
            HttpResponseMessage response = client.PutAsJsonAsync("api/players/foo", player).Result;

            //get the token again as it will include the gamertag claim
            tokenResponse = tokenClient.RequestResourceOwnerPasswordAsync(username, password, "nether-all").Result;
            client.SetBearerToken(tokenResponse.AccessToken);
            token = new JwtSecurityToken(tokenResponse.AccessToken);

            return client;
        }
    }
}
