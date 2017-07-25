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
                app.Command("facebook-token", "Test facebook user access token flow", new FacebookUserTokenCommand(app));
                app.Command("guest", "Test guest flow", new GuestAuthCommand(app));

                app.StandardHelpOption();
                app.ShowHelpOnExecute();
                app.Execute(args);
            }
            catch (CommandParsingException cpe)
            {
                Console.WriteLine($"Error parsing: {cpe.Message}");
            }
        }
    }
}
