using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServerTestClient
{
    public class IdentityClientApplication : CommandLineApplication
    {
        public GlobalArguments GlobalArguments { get; private set; }

        public string IdentityRootUrl => GlobalArguments.RootUrl.Value() ?? "http://localhost:5000/identity";

        public IdentityClientApplication()
        {
            GlobalArguments = new GlobalArguments
            {
                RootUrl = Option("--identity-root-url", "Root Url for the identity service", CommandOptionType.SingleValue)
            };
        }

    }

    public  class GlobalArguments
    {
        public CommandOption RootUrl { get; internal set; }
    }
}
