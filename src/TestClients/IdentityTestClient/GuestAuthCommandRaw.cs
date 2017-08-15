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
    /// A command to test/demonstrate the custom guest authentication flow (using raw HttpClient calls)
    /// </summary>
    internal class GuestAuthRawCommand : CommandBase
    {
        private CommandOption _clientIdOption;
        private CommandOption _clientSecretOption;
        private CommandOption _guestIdentifierOption;

        public GuestAuthRawCommand(IdentityClientApplication application)
            : base(application)
        {
        }

        public override void Register(CommandLineApplication config)
        {
            base.Register(config);

            _clientIdOption = config.Option("--client-id", "clientid", CommandOptionType.SingleValue);
            _clientSecretOption = config.Option("--client-secret", "clientsecret", CommandOptionType.SingleValue);
            _guestIdentifierOption = config.Option("--guest-id", "Guest identifier", CommandOptionType.SingleValue);

            config.StandardHelpOption();
        }


        protected override async Task<int> ExecuteAsync()
        {
            var clientId = _clientIdOption.GetValue("client-id", requireNotNull: true, promptIfNull: true);
            var clientSecret = _clientSecretOption.GetValue("client-secret", requireNotNull: true, promptIfNull: true, sensitive: true);
            var guestIdentifier = _guestIdentifierOption.GetValue("guest identifier", requireNotNull: true, promptIfNull: true);


            string rootUrl = Application.IdentityRootUrl;

            var client = new HttpClient();
            var requestBody = new FormUrlEncodedContent(
                   new Dictionary<string, string>
                   {
                        { "guest_identifier", guestIdentifier },
                        { "grant_type", "guest-access" },
                        { "client_id",  clientId },
                        { "client_secret", clientSecret },
                        { "scope", "nether-all" }
                   }
               );

            var response = await client.PostAsync($"{rootUrl}connect/token", requestBody);
            dynamic responseBody = await response.Content.ReadAsAsync<dynamic>();

            if (responseBody.error != null)
            {
                Console.WriteLine((string)responseBody.Error);
                return -1;
            }

            Console.WriteLine("Token response:");
            Console.WriteLine(responseBody.ToString());
            Console.WriteLine("\n\n");
            string access_token = responseBody.access_token;

            Console.WriteLine("Calling echo API:");
            await EchoClaimsAsync(access_token);
            Console.WriteLine("\n\n");

            Console.WriteLine("Checking role:");
            await ShowPlayerInfoAsync(access_token);
            Console.WriteLine("\n\n");

            return 0;
        }
    }
}
