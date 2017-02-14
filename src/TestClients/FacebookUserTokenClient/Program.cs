// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FacebookUserTokenClient
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            MainAsync().Wait();
        }
        public static async Task MainAsync()
        {
            var baseUrl = "http://localhost:5000";

            var client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };

            Console.WriteLine("Enter the Facebook User Token (see https://developers.facebook.com/tools/accesstoken):");
            string fbToken = Console.ReadLine();
            Console.WriteLine();

            var accessTokenResult = await client.GetAccessTokenAsync(fbToken);

            if (accessTokenResult.Error != null)
            {
                Console.WriteLine($"Error: {accessTokenResult.Error}");
                return;
            }
            Console.WriteLine($"Access token: {accessTokenResult.AccessToken}");
            Console.WriteLine();

            var accessToken = accessTokenResult.AccessToken;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Get player info
            var user = await GetPlayerInfoAsync(client);
            if (user == null)
            {
                Console.WriteLine("No player info... enter gamertag");
                var gamertag = Console.ReadLine();

                await SetPlayerInfoASync(client, gamertag);

                // refresh access token with gamertag
                accessTokenResult = await client.GetAccessTokenAsync(fbToken);

                if (accessTokenResult.Error != null)
                {
                    Console.WriteLine($"Error: {accessTokenResult.Error}");
                    return;
                }
                Console.WriteLine($"Access token: {accessTokenResult.AccessToken}");
                Console.WriteLine();

                accessToken = accessTokenResult.AccessToken;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            user = await GetPlayerInfoAsync(client);
            Console.WriteLine();
            Console.WriteLine($"user: {user}");

            Console.WriteLine("Enter your score");
            int score = int.Parse(Console.ReadLine());
            Console.WriteLine();
            Console.WriteLine("Posting score {0}", score);
            await client.PostScoreAsync(accessTokenResult.AccessToken, score);
        }


        public static async Task PostScoreAsync(this HttpClient client, string bearerToken, int score)
        {
            var response = await client.PostAsJsonAsync("/api/leaderboard", new { country = "missing", customTag = "testclient", score = score });

            response.EnsureSuccessStatusCode();
        }

        public static async Task<AccessTokenResult> GetAccessTokenAsync(this HttpClient client, string facebookUserAccessToken)
        {
            const string client_id = "devclient";
            const string client_secret = "devsecret";
            const string scope = "openid profile nether-all";

            var requestBody = new FormUrlEncodedContent(
                   new Dictionary<string, string>
                   {
                        { "token", facebookUserAccessToken },
                        { "grant_type", "fb-usertoken" },
                        { "client_id",  client_id },
                        { "client_secret", client_secret },
                        { "scope", scope }
                   }
               );
            var response = await client.PostAsync("/identity/connect/token", requestBody);
            dynamic responseBody = await response.Content.ReadAsAsync<dynamic>();

            if (responseBody.error != null)
            {
                return new AccessTokenResult { Error = responseBody.Error };
            }
            Console.WriteLine(responseBody);

            var access_token = (string)responseBody.access_token;
            if (string.IsNullOrWhiteSpace(access_token))
            {
                response.EnsureSuccessStatusCode();
            }
            return new AccessTokenResult
            {
                AccessToken = access_token
            };
        }

        private static async Task<dynamic> GetPlayerInfoAsync(HttpClient client)
        {
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
        private static async Task SetPlayerInfoASync(HttpClient client, string gamertag)
        {
            // call api

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

        public class AccessTokenResult
        {
            public string Error { get; set; }
            public string AccessToken { get; set; }
        }
    }
}
