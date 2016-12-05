using System;
using System.Collections.Generic;
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
        protected const string Username = "testuser";
        protected const string GamerTag = "testusertag";
        private const string Password = "testuser";
        private static HttpClient s_client; //create once to avoid authentication overhead for integration tests

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

        protected static HttpClient Client
        {
            get
            {
                if(s_client == null)
                {
                    s_client = CreateClient(BaseUrl);

                    //authenticate so it's ready for use
                    DiscoveryResponse disco = DiscoveryClient.GetAsync(BaseUrl).Result;
                    if (disco.TokenEndpoint == null)
                    {
                        throw new AuthenticationException("could not discover endpoint, server is offline?");
                    }

                    var tokenClient = new TokenClient(disco.TokenEndpoint, ClientId, ClientSecret);
                    TokenResponse tokenResponse = tokenClient.RequestResourceOwnerPasswordAsync(Username, Password, "nether-all").Result;

                    if(tokenResponse.IsError)
                    {
                        throw new AuthenticationException("filed to authenticate");
                    }

                    s_client.SetBearerToken(tokenResponse.AccessToken);
                }

                return s_client;
            }
        }
    }
}
