// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using IdentityModel;
using Microsoft.Extensions.CommandLineUtils;

namespace IdentityServerTestClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var app = new IdentityClientApplication();
                app.Command("client-creds", "Test client credential flow", new ClientCredentialsCommand(app));
                app.Command("resource-owner", "Test resource owner (username + password) flow", new ResourceOwnerPasswordCommand(app));

                app.StandardHelpOption();
                app.ShowHelpOnExecute();
                app.Execute(args);
            }
            catch (CommandParsingException cpe)
            {
                Console.WriteLine($"Error parsing: {cpe.Message}");
            }
            return;

            //MainAsync().Wait();
        }
        public static async Task MainAsync()
        {
            try
            {
                Console.WriteLine("Enter the Facebook User Token (see https://developers.facebook.com/tools/accesstoken):");
                var facebookUserToken = Console.ReadLine();

                Console.WriteLine("Getting token..");
                var tokenResponse = await TestCustomGrantAsync(facebookUserToken); // this flow uses the custom grant that takes the facebook user access token (as obtained via the unity plugin, or fb developer site!)

                var accessToken = tokenResponse.AccessToken;
                if (accessToken == null)
                {
                    Console.WriteLine("No token");
                    return;
                }
                var gamertagFromToken = GetGamerTagFromAccessToken(accessToken);
                Console.WriteLine($"Gamertag from token: {gamertagFromToken}");

                if (gamertagFromToken == null)
                {
                    Console.WriteLine();
                    Console.WriteLine("New player... enter gamertag:");
                    var gamertag = Console.ReadLine();
                    await SetPlayerInfoAsync(accessToken, gamertag);

                    // refresh access token to include gamertag
                    Console.WriteLine("Getting token (after setting gamertag)...");
                    tokenResponse = await TestCustomGrantAsync(facebookUserToken); // this flow uses the custom grant that takes the facebook user access token (as obtained via the unity plugin, or fb developer site!)
                    accessToken = tokenResponse.AccessToken;
                    gamertagFromToken = GetGamerTagFromAccessToken(accessToken);
                    Console.WriteLine($"Gamertag from token: {gamertagFromToken}");
                }

                //Console.WriteLine();
                //Console.WriteLine("Dump claims:");
                //await CallApiAsync(accessToken);

                Console.WriteLine();
                Console.WriteLine("Player info:");
                var user = await GetPlayerInfoAsync(accessToken);
                Console.WriteLine(user);

                Console.WriteLine();
                Console.WriteLine("Post score...");
                await PostScoreAsync(accessToken, 13);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static string GetGamerTagFromAccessToken(string accessToken)
        {
            var token = new JwtSecurityToken(accessToken);
            var claim = token.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.NickName);
            var nickname = claim?.Value;
            return nickname;
        }

        private static async Task<TokenResponse> TestCustomGrantAsync(string facebookUserToken)
        {
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, "customgranttest", "devsecret");


            var tokenResponse = await tokenClient.RequestCustomGrantAsync("fb-usertoken", "nether-all", new { token = facebookUserToken });

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

        private static async Task<dynamic> GetPlayerInfoAsync(string accessToken)
        {
            // call api
            var client = new HttpClient();
            client.SetBearerToken(accessToken);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.GetAsync("http://localhost:5000/api/player");
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }

            var content = response.Content.ReadAsStringAsync().Result;
            dynamic result = JToken.Parse(content);
            return result.player;
        }
        private static async Task SetPlayerInfoAsync(string accessToken, string gamertag)
        {
            // call api
            var client = new HttpClient();
            client.SetBearerToken(accessToken);

            var response = await client.PutAsJsonAsync("http://localhost:5000/api/player",
               new
               {
                   Country = "missing",
                   Gamertag = gamertag
               });
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
        }

        public static async Task PostScoreAsync(string accessToken, int score)
        {
            var client = new HttpClient();
            client.SetBearerToken(accessToken);
            var response = await client.PostAsJsonAsync("http://localhost:5000/api/scores", new { country = "missing", score = score });

            response.EnsureSuccessStatusCode();
        }
    }
}
