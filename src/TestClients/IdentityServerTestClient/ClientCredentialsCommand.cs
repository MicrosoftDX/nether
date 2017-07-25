using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;

namespace IdentityServerTestClient
{
    class ClientCredentialsCommand : CommandBase
    {
        private CommandOption _clientIdOption;
        private CommandOption _clientSecretOption;
        public ClientCredentialsCommand(IdentityClientApplication application)
            : base(application)
        {

        }
        public override void Register(CommandLineApplication config)
        {
            base.Register(config);

            _clientIdOption = config.Option("--client-id", "clientid", CommandOptionType.SingleValue);
            _clientSecretOption = config.Option("--client-secret", "clientsecret", CommandOptionType.SingleValue);

            config.StandardHelpOption();
        }
        protected override async Task<int> ExecuteAsync()
        {
            var clientId = _clientIdOption.Value();
            if (string.IsNullOrEmpty(clientId))
            {
                Console.WriteLine("client-id is required");
                return -1;
            }
            var clientSecret = _clientSecretOption.Value();
            if (string.IsNullOrEmpty(clientSecret))
            {
                Console.WriteLine("client-secret is required");
                return -1;
            }

            string rootUrl = Application.IdentityRootUrl;
            var disco = await DiscoveryClient.GetAsync(rootUrl);

            if (string.IsNullOrEmpty(disco.TokenEndpoint))
            {
                Console.WriteLine($"Unable to discover token endpoint from '{rootUrl}' - is the server online?");
                return -1;
            }

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, clientId, clientSecret);
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

            return 0;
        }
    }
}
