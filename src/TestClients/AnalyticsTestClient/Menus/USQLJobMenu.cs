// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest.Azure.Authentication;
using Nether.Analytics;
using Nether.Analytics.DataLake;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AnalyticsTestClient
{
    public class USQLJobMenu : ConsoleMenu
    {
        public USQLJobMenu()
        {
            Title = "Run Custom USQL scripts";
            MenuItems.Add('1', new ConsoleMenuItem("Run", async () => { await RunUSQLScriptFlowAsync(); }));
        }

        private async Task RunUSQLScriptFlowAsync()
        {
            var variables = new Dictionary<string, object>();

            var jobName = ConsoleEx.ReadLine("Job Name", "MyDlaJob");
            var path = ConsoleEx.ReadLine("Path to U-SQL File", @"c:\tmp\job.usql", s => File.Exists(s));
            var script = File.ReadAllText(path);

            Console.WriteLine("Parameters");

            while (true)
            {
                var name = ConsoleEx.ReadLine("Parameter <Blank to Continue>");
                if (string.IsNullOrWhiteSpace(name))
                    break;

                var val = ConsoleEx.ReadLine($"Value for @{name}");

                variables.Add(name, val);
            }

            // Authenticate against Azure AD once and re-use for all needed purposes
            var serviceClientCredentials = await ApplicationTokenProvider.LoginSilentAsync(
                Config.Root[Config.NAH_AAD_Domain],
                new ClientCredential(Config.Root[Config.NAH_AAD_CLIENTID], Config.Root[Config.NAH_AAD_CLIENTSECRET]));

            var jm = new DataLakeAnalyticsJobManager(serviceClientCredentials,
               Config.Root[Config.NAH_AZURE_SUBSCRIPTIONID], Config.Root[Config.NAH_AZURE_DLA_ACCOUNTNAME]);
            var jobID = await jm.SubmitJobAsync(jobName, script, variables);
            await jm.WaitForJobAsync(jobID);
        }
    }
}
