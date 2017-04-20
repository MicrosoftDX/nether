// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Net.Http;
using System.Net;
using IdentityModel.Client;
using System.Security.Authentication;
using Microsoft.VisualStudio.TestTools.LoadTesting;
using NetherLoadTest.Helpers;
using System.Configuration;
using Nether.Sdk;

namespace NetherLoadTest
{
    [TestClass]
    public class NetherUnitTest
    {
        private NetherClient _client;
        private static Random s_random = new Random();

        public TestContext TestContext { get; set; }



        ////////////////////////////////////////////////////////
        // Read-only properties that expose the load test config

        public string BaseUrl
        {
            get { return ConfigurationManager.AppSettings["BaseUrl"].ToString(); }
        }
        public string ClientId
        {
            get { return ConfigurationManager.AppSettings["ClientId"].ToString(); }
        }
        public string ClientSecret
        {
            get { return ConfigurationManager.AppSettings["ClientSecret"].ToString(); }
        }
        public string AdminUserName
        {
            get { return ConfigurationManager.AppSettings["AdminUserName"].ToString(); }
        }
        public string AdminPassword
        {
            get { return ConfigurationManager.AppSettings["AdminPassword"].ToString(); }
        }


        /////////////////////////////////////////////
        // Test properties that store the test state
        private LoadTestUserContext UserContext
        {
            get
            {
                return TestContext.Properties["$LoadTestUserContext"] as LoadTestUserContext;
            }
        }
        // Saving the access token as we can't serialise the NetherClient
        // Storing this allows us to pick up the access token for the virtual user without logging in again
        public string AccessToken
        {
            get { return UserContext.GetWithDefault("Test_AccessToken", (string)null); }
            set { UserContext["Test_AccessToken"] = value; }
        }
        public bool LoggedIn
        {
            get { return UserContext.GetWithDefault("Test_LoggedIn", false); }
            set { UserContext["Test_LoggedIn"] = value; }
        }

        public string UserName
        {
            get { return UserContext.GetWithDefault("Test_UserName", (string)null); }
            set { UserContext["Test_UserName"] = value; }
        }
        public string Password
        {
            get { return UserContext.GetWithDefault("Test_Password", (string)null); }
            set { UserContext["Test_Password"] = value; }
        }

        public int UserId
        {
            get { return UserContext.UserId; }
        }

        [TestInitialize]
        public void Init()
        {
            if (UserName == null)
            {
                UserName = "loadUser_" + UserId; // hard coded user names created for the load test in the memory store
                Password = UserName;
            }

            var baseUrl = BaseUrl ?? "http://localhost:5000";
            _client = new NetherClient(baseUrl, ClientId, ClientSecret);
            if (LoggedIn)
            {
                // we've already logged in so set the access token
                _client.AccessToken = AccessToken;
            }
        }

        [TestMethod]
        public async Task PostScoreAsync()
        {
            await EnsureLoggedInAsync();

            TestContext.BeginTimer("PostScore");
            await _client.PostScoreAsync(s_random.Next(100, 1000));
            TestContext.EndTimer("PostScore");
        }

        [TestMethod]
        public async Task GetScore()
        {
            await EnsureLoggedInAsync();

            TestContext.BeginTimer("GetScore");
            await _client.GetScoresAsync();
            TestContext.EndTimer("GetScore");
        }

        [TestMethod]
        public async Task PlayGame()
        {
            // simuate game:
            // 1. log in
            // 2. get top 10 leaderboard 
            // 3. wait random time
            // 4. post new score
            // 5. get top 10 leaderboard

            await EnsureLoggedInAsync();

            TestContext.BeginTimer("GetScore");
            await _client.GetScoresAsync("Top_10");
            TestContext.EndTimer("GetScore");

            // sleep between 30 seconds to 5 minutes
            Thread.Sleep(s_random.Next(30, 300) * 1000);

            TestContext.BeginTimer("PlayLevelPostScore");
            await _client.PostScoreAsync(s_random.Next(100, 1000));
            TestContext.EndTimer("PlayLevelPostScore");

            TestContext.BeginTimer("PlayLevelGetScore");
            await _client.GetScoresAsync("Top_10");
            TestContext.EndTimer("PlayLevelGetScore");
        }

        [TestMethod]
        public async Task StressPlayGame()
        {
            // simuate game:
            // 1. log in
            // 2. get top 10 leaderboard 
            // 3. wait 10 seconds
            // 4. post new score
            // 5. get top 10 leaderboard

            await EnsureLoggedInAsync();

            TestContext.BeginTimer("GetScore");
            await _client.GetScoresAsync("Top_10");
            TestContext.EndTimer("GetScore");

            // sleep for 10 seconds
            Thread.Sleep(10 * 1000);

            TestContext.BeginTimer("PlayLevelPostScore");
            await _client.PostScoreAsync(s_random.Next(100, 1000));
            TestContext.EndTimer("PlayLevelPostScore");

            TestContext.BeginTimer("PlayLevelGetScore");
            await _client.GetScoresAsync("Top_10");
            TestContext.EndTimer("PlayLevelGetScore");
        }

        private async Task EnsureLoggedInAsync()
        {
            if (!LoggedIn)
            {
                // ensure that the user exists!
                await EnsureUserExistsAsync();

                // log in
                await _client.LoginUserNamePasswordAsync(UserName, Password);

                // save the access token
                AccessToken = _client.AccessToken;
                LoggedIn = true;
            }
        }
        private async Task EnsureUserExistsAsync()
        {
            // Log in as admin
            var adminClient = await GetClientAsync(AdminUserName, AdminPassword);

            // check if the user already exist            
            var response = await adminClient.GetAsync($"/api/identity/users/{UserName}");
            // 404 - user not found - create the user         
            if (response.StatusCode.Equals(HttpStatusCode.NotFound))
            {
                TestContext.WriteLine($"User {UserName} does not exist. Creating...");

                // Create the user
                response = await adminClient.PutAsJsonAsync(
                    $"/api/identity/users/{UserName}",
                    new
                    {
                        role = "Player",
                        active = true
                    });
                response.EnsureSuccessStatusCode();

                // create login
                response = await adminClient.PutAsJsonAsync(
                    $"/api/identity/users/{UserName}/logins/password/{UserName}", // reuse username as gamertag
                    new
                    {
                        Password
                    });

                // sign in as player and create gamertag - assuming the password did not change
                var playerClient = await GetClientAsync(UserName, Password);
                var player = new
                {
                    gamertag = UserName,
                    country = "UK",
                    customTag = "LoadTestUser"
                };

                response = await playerClient.PutAsJsonAsync("api/player", player);
                response.EnsureSuccessStatusCode();
            }
        }

        private async Task<HttpClient> GetClientAsync(string username, string password)
        {
            var discoveryResponse = await DiscoveryClient.GetAsync(BaseUrl + "/identity");
            if (discoveryResponse.TokenEndpoint == null)
            {
                throw new AuthenticationException("GetClient: could not discover endpoint, server is offline?");
            }

            var tokenClient = new TokenClient(discoveryResponse.TokenEndpoint, ClientId, ClientSecret);
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(username, password, "nether-all");
            if (tokenResponse.IsError)
            {
                throw new AuthenticationException($"GetClient: failed to authenticate: '{tokenResponse.ErrorDescription}'");
            }

            // Create HttpClient with admin token
            var client = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };

            client.SetBearerToken(tokenResponse.AccessToken);
            return client;
        }
    }
}
