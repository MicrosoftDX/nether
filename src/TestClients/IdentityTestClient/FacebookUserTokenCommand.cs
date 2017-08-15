// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.CommandLineUtils;
using System.Threading.Tasks;
using IdentityModel.Client;
using System.Net.Http;

namespace IdentityTestClient
{
    /// <summary>
    /// A command to test/demonstrate the custom facebook user access token flow (using IdentityModel.Client)
    /// </summary>
    internal class FacebookUserTokenCommand : CommandBase
    {
        private CommandOption _clientIdOption;
        private CommandOption _clientSecretOption;
        private CommandOption _facebookTokenOption;

        public FacebookUserTokenCommand(IdentityClientApplication application)
            : base(application)
        {
        }

        public override void Register(CommandLineApplication config)
        {
            base.Register(config);

            _clientIdOption = config.Option("--client-id", "clientid", CommandOptionType.SingleValue);
            _clientSecretOption = config.Option("--client-secret", "clientsecret", CommandOptionType.SingleValue);
            _facebookTokenOption = config.Option("--facebook-token", "facebook user token - see https://developers.facebook.com/tools/accesstoken", CommandOptionType.SingleValue);

            config.StandardHelpOption();
        }


        protected override async Task<int> ExecuteAsync()
        {
            var clientId = _clientIdOption.GetValue("client-id", requireNotNull: true, promptIfNull: true);
            var clientSecret = _clientSecretOption.GetValue("client-secret", requireNotNull: true, promptIfNull: true, sensitive: true);
            var facebookUserToken = _facebookTokenOption.GetValue("facebook-token", requireNotNull: true, promptIfNull: true, additionalPromptText: " (see https://developers.facebook.com/tools/accesstoken)");

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
            var tokenResponse = await tokenClient.RequestCustomGrantAsync("fb-usertoken", "nether-all", new { token = facebookUserToken });


            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return -1;
            }

            Console.WriteLine("Token response:");
            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            Console.WriteLine("Calling echo API:");
            await EchoClaimsAsync(tokenResponse.AccessToken);
            Console.WriteLine("\n\n");

            Console.WriteLine("Checking role:");
            await ShowPlayerInfoAsync(tokenResponse.AccessToken);
            Console.WriteLine("\n\n");
            return 0;
        }
    }
}
