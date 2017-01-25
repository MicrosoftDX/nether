using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nether.Web.IntegrationTests
{
    public class IntegrationTestUsersFixture : WebTestBase, IDisposable
    {
        private const string InitialAdminUserName = "netheradmin";
        private const string InitialAdminPassword = "N3therAdm1n";
        private List<string> _createdUserNames = new List<string>();
        private List<string> _createdPlayers = new List<string>();

        public IntegrationTestUsersFixture()
        {
            foreach(var user in s_users)
            {
                CreateUser(user.UserName, user.Password, user.Role, user.Gamertag);
            }
        }


        public void Dispose()
        {
            foreach (var username  in _createdUserNames)
            {
                DeleteUser(username);
            }
            foreach (var gamertag in _createdPlayers)
            {
                DeletePlayer(gamertag);
            }
        }


        public void CreateUser(string username, string password, string role, string gamertag)
        {
            Task.Run(()=>CreateUserAsync(username, password, role, gamertag)).Wait();
        }
        public async Task CreateUserAsync(string username, string password, string role, string gamertag)
        {
            var client = await GetClientAsync(InitialAdminUserName, InitialAdminPassword).ConfigureAwait(false);

            // create user 
            var response = await client.PutAsJsonAsync(
                $"/api/identity/users/{username}",
                new
                {
                    role = role,
                    active = true
                });
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsAsync<dynamic>();
            _createdUserNames.Add(username);

            // create login
            response = await client.PutAsJsonAsync(
                $"/api/identity/users/{username}/logins/password/{username}",
                new
                {
                    password
                });
            response.EnsureSuccessStatusCode();

            if (!string.IsNullOrEmpty(gamertag))
            {
                // sign in as player and create gamertag
                var playerClient = await GetClientAsync(username);
                var player = new
                {
                    gamertag,
                    country = "UK",
                    customTag = "IntegrationTestUser"
                };
                response = await playerClient.PutAsJsonAsync("api/player", player);
                response.EnsureSuccessStatusCode();

                _createdPlayers.Add(gamertag);
            }
        }

        public void DeleteUser(string username)
        {
            Task.Run(() => DeleteUserAsync(username)).Wait();
        }
        public async Task DeleteUserAsync(string username)
        {
            var client = await GetClientAsync(InitialAdminUserName,InitialAdminPassword).ConfigureAwait(false);

            await client.DeleteAsync($"/api/identity/users/{username}").ConfigureAwait(false);
        }
        public void DeletePlayer(string gamertag)
        {
            Task.Run(() => DeletePlayerAsync(gamertag)).Wait();
        }
        public async Task DeletePlayerAsync(string gamertag)
        {
            var client = await GetClientAsync(InitialAdminUserName, InitialAdminPassword).ConfigureAwait(false);

            await client.DeleteAsync($"/api/players/{gamertag}").ConfigureAwait(false);
        }
    }
}
