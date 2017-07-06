// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GuestAccessTestClient
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

            Console.WriteLine("Enter the guest token:");
            string guestToken = Console.ReadLine();
            Console.WriteLine();

            var accessTokenResult = await client.GetAccessTokenAsync(guestToken);

            if (accessTokenResult.Error != null)
            {
                Console.WriteLine($"Error: {accessTokenResult.Error}");
                return;
            }
            Console.WriteLine($"Access token: {accessTokenResult.AccessToken}");
            Console.WriteLine();

            var accessToken = accessTokenResult.AccessToken;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public static async Task<AccessTokenResult> GetAccessTokenAsync(this HttpClient client, string guestAccessToken)
        {
            const string client_id = "devclient";
            const string client_secret = "devsecret";
            const string scope = "openid profile nether-all";

            var requestBody = new FormUrlEncodedContent(
                   new Dictionary<string, string>
                   {
                        { "token", guestAccessToken },
                        { "grant_type", "guest-access" },
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

        public class AccessTokenResult
        {
            public string Error { get; set; }
            public string AccessToken { get; set; }
        }
    }
}