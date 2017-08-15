// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using IdentityModel;
using System.Net.Http.Headers;
using System.Net;

namespace IdentityTestClient
{
    public abstract class CommandBase
    {
        public CommandLineApplication CommandConfig { get; private set; }
        public IdentityClientApplication Application { get; private set; }

        public CommandBase(IdentityClientApplication application)
        {
            Application = application;
        }
        public virtual void Register(CommandLineApplication config)
        {
            CommandConfig = config;
            config.OnExecute(() => WrappedExecuteAsync());
        }
        private async Task<int> WrappedExecuteAsync()
        {
            try
            {
                return await ExecuteAsync();
            }
            catch (CommandArgumentException cae)
            {
                Console.WriteLine(cae.Message);
                return -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return -1;
            }
        }
        protected abstract Task<int> ExecuteAsync();


        protected async Task EchoClaimsAsync(string accessToken)
        {
            // call api
            var client = new HttpClient();
            client.SetBearerToken(accessToken);

            var response = await client.GetAsync($"{Application.ApiRootUrl}identity-test");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(JObject.Parse(content));
        }
        protected async Task ShowPlayerInfoAsync(string accessToken)
        {
            var role = GetClaimFromToken(accessToken, JwtClaimTypes.Role);
            Console.WriteLine($"Role in token is '{role}'");
            if (role == "Player")
            {
                var player = await GetPlayerInfoAsync(accessToken);
                Console.WriteLine(player.ToString());
            }
        }

        protected async Task<dynamic> GetPlayerInfoAsync(string accessToken)
        {
            // call api
            var client = new HttpClient();
            client.SetBearerToken(accessToken);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.GetAsync($"{Application.ApiRootUrl}/player");
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

        protected string GetClaimFromToken(string accessToken, string claimType)
        {
            return new JwtSecurityToken(accessToken)
                .Claims
                .FirstOrDefault(c => c.Type == claimType)
                ?.Value;
        }
    }
}