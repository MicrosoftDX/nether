using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace IdentityServerTestClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MainAsync().Wait();
        }
        public static async Task MainAsync()
        {
            try
            {
                //var tokenResponse = await TestClientCredentialsAsync(); // this flow just identifies the client app
                //var tokenResponse = await TestResourceOwnerPasswordAsync(); // this flow uses username + password
                var tokenResponse = await TestCustomGrantAsync(); // this flow uses the custom grant that takes the facebook user access token (as obtained via the unity plugin, or fb developer site!)


                var accessToken = tokenResponse.AccessToken;
                await CallApiAsync(accessToken);
                await PostScoreAsync(accessToken, 13);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static async Task<TokenResponse> TestCustomGrantAsync()
        {
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, "customgrant-test", "devsecret");
            Console.WriteLine("Enter the Facebook User Token (see https://developers.facebook.com/tools/accesstoken):");
            var token = Console.ReadLine();

            var tokenResponse = await tokenClient.RequestCustomGrantAsync("fb-usertoken", "nether-all", new { token = token });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
            }
            else
            {
                Console.WriteLine(tokenResponse.Json);
                Console.WriteLine("\n\n");
            }

            return tokenResponse;
        }
        private static async Task<TokenResponse> TestResourceOwnerPasswordAsync()
        {
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, "resourceowner-test", "devsecret");
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("devuser", "devuser", "nether-all");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
            }
            else
            {
                Console.WriteLine(tokenResponse.Json);
                Console.WriteLine("\n\n");
            }

            return tokenResponse;
        }
        private static async Task<TokenResponse> TestClientCredentialsAsync()
        {
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, "clientcreds-test", "devsecret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("nether-all");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
            }
            else
            {
                Console.WriteLine(tokenResponse.Json);
                Console.WriteLine("\n\n");
            }

            return tokenResponse;
        }

        private static async Task CallApiAsync(string accessToken)
        {
            // call api
            var client = new HttpClient();
            client.SetBearerToken(accessToken);

            var response = await client.GetAsync("http://localhost:5000/identity-test");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }

            var content = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(JArray.Parse(content));
        }

        public static async Task PostScoreAsync(string accessToken, int score)
        {
            var client = new HttpClient();
            client.SetBearerToken(accessToken);
            var response = await client.PostAsJsonAsync("http://localhost:5000/api/leaderboard", new { country = "missing", customTag = "testclient", score = score });

            response.EnsureSuccessStatusCode();
        }
    }
}
