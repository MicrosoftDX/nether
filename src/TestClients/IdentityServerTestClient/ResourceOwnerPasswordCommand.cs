﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.CommandLineUtils;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace IdentityServerTestClient
{
    class ResourceOwnerPasswordCommand : CommandBase
    {
        private CommandOption _clientIdOption;
        private CommandOption _clientSecretOption;
        private CommandOption _usernameOption;
        private CommandOption _passwordOption;

        public ResourceOwnerPasswordCommand(IdentityClientApplication application)
            : base(application)
        {

        }

        public override void Register(CommandLineApplication config)
        {
            base.Register(config);

            _clientIdOption = config.Option("--client-id", "clientid", CommandOptionType.SingleValue);
            _clientSecretOption = config.Option("--client-secret", "clientsecret", CommandOptionType.SingleValue);
            _usernameOption = config.Option("--username", "username", CommandOptionType.SingleValue);
            _passwordOption = config.Option("--password", "user password", CommandOptionType.SingleValue);

            config.StandardHelpOption();
        }


        protected override async Task<int> ExecuteAsync()
        {
            var clientId = _clientIdOption.GetValue("client-id", requireNotNull:true, promptIfNull:true);
            var clientSecret = _clientSecretOption.GetValue("client-secret", requireNotNull:true, promptIfNull:true, sensitive:true);
            var username = _usernameOption.GetValue("username", requireNotNull:true, promptIfNull:true);
            var password = _passwordOption.GetValue("password", requireNotNull: true, promptIfNull:true, sensitive:true);


            string rootUrl = Application.IdentityRootUrl;
            var disco = await DiscoveryClient.GetAsync(rootUrl);

            if (string.IsNullOrEmpty(disco.TokenEndpoint))
            {
                Console.WriteLine($"Unable to discover token endpoint from '{rootUrl}' - is the server online?");
                return -1;
            }

            if (string.IsNullOrEmpty(disco.TokenEndpoint))
            {
                Console.WriteLine($"Unable to discover token endpoint from '{rootUrl}' - is the server online?");
                return -1;
            }

            var tokenClient = new TokenClient(disco.TokenEndpoint, clientId, clientSecret);
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(username, password, "nether-all");


            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return -1;
            }

            Console.WriteLine("Token response:");
            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            await EchoClaimsAsync(tokenResponse.AccessToken);

            return 0;

        }



    }
}
