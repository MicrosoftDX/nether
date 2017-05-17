using AnalyticsTestClient.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest.Azure.Authentication;
using Nether.Analytics.DataLake;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AnalyticsTestClient
{
    public class USQLJobMenu : ConsoleMenu
    {
        public USQLJobMenu()
        {
            Title = "Run Custom USQL scripts";
            SetupConfigurationProviders();
            MenuItems.Add('1', new ConsoleMenuItem("Run", async () => { await RunUSQLScriptFlowAsync(); }));
        }
        private const string AppSettingsFile = "appsettings.json";
        private const string NAH_AAD_Domain = "NAH_AAD_DOMAIN";
        private const string NAH_AAD_ClientId = "NAH_AAD_CLIENTID";
        private const string NAH_AAD_ClientSecret = "NAH_AAD_CLIENTSECRET";
        private const string NAH_Azure_DLA_AccountName = "NAH_AZURE_DLA_ACCOUNTNAME";
        private const string NAH_Azure_SubscriptionId = "NAH_AZURE_SUBSCRIPTIONID";

        private const string NAH_Azure_DLSOutputManager_AccountName = "NAH_AZURE_DLSOUTPUTMANAGER_ACCOUNTNAME";
        private IConfigurationRoot _configuration;
        private async Task RunUSQLScriptFlowAsync()
        {
            string script = string.Empty;
            var variables = new Dictionary<string, object>();
            string jobname = "My DLA Job";
            Console.Write($"Job name [My DLA Job]: ");
            string _jobname = Console.ReadLine();
            if (!string.IsNullOrEmpty(_jobname.Trim()))
            {
                jobname = _jobname;
            }

            bool fileFound = false;
            do
            {
                Console.Write("Path to the USQL file: ");
                string path = Console.ReadLine();

                try
                {
                    script = File.ReadAllText(path);
                }
                catch
                {
                    fileFound = false;
                }

                if (string.IsNullOrEmpty(script)) fileFound = false;
                else fileFound = true;

                if (!fileFound) Console.Write($"Error reading {path} - ");
            } while (!fileFound);

            Console.WriteLine("Let's enter custom variables for the script. Enter a blank string for the name to stop");

            bool completed = false;
            do
            {
                Console.Write("Variable name: ");
                string name = Console.ReadLine();
                if (string.IsNullOrEmpty(name.Trim())) { completed = true; }
                else
                {
                    Console.Write($"Value for {name}: ");
                    string value = Console.ReadLine();
                    Console.WriteLine($"Declared with name {name} and value {value}");
                    variables.Add(name, value);
                }
            } while (!completed);

            var configSet = IsConfigSetup(
               NAH_AAD_Domain,
               NAH_AAD_ClientId,
               NAH_AAD_ClientSecret,
               NAH_Azure_SubscriptionId,
               NAH_Azure_DLSOutputManager_AccountName);

            if (!configSet)
            {
                // Exiting due to missing configuration
                Console.WriteLine("Press any key to continue");
                Console.ReadKey(true);
                return;
            }

            // Authenticate against Azure AD once and re-use for all needed purposes
            var serviceClientCredentials = await ApplicationTokenProvider.LoginSilentAsync(_configuration[NAH_AAD_Domain],
                new ClientCredential(_configuration[NAH_AAD_ClientId], _configuration[NAH_AAD_ClientSecret]));

            var jm = new DataLakeAnalyticsJobManager(serviceClientCredentials,
               _configuration[NAH_Azure_SubscriptionId], _configuration[NAH_Azure_DLA_AccountName]);
            var jobID = await jm.SubmitJobAsync(jobname, script, variables);
            await jm.WaitForJobAsync(jobID);
        }

        private bool IsConfigSetup(params string[] settings)
        {
            const int maxValueLengthPrinted = 100;

            Console.WriteLine("Using the following configuration values:");
            Console.WriteLine();

            var missingSettings = new List<string>();

            foreach (var setting in settings)
            {
                var val = _configuration[setting];

                if (string.IsNullOrWhiteSpace(val))
                {
                    missingSettings.Add(setting);
                }
                else
                {
                    Console.Write(setting);
                    Console.WriteLine(" : ");
                    Console.WriteLine("  " + (val.Length < maxValueLengthPrinted ? val : val.Substring(0, maxValueLengthPrinted - 3) + "..."));
                }
            }

            Console.WriteLine();

            if (missingSettings.Count > 0)
            {
                Console.WriteLine("The following setting(s) are missing values:");
                Console.WriteLine();


                foreach (var setting in missingSettings)
                {
                    Console.WriteLine($"  {setting}");
                }

                Console.WriteLine();
                Console.WriteLine($"Make sure to set all the above configuration parameters in {AppSettingsFile} or using Environment Variables.");
                Console.WriteLine("Then start Nether.Analytics.Host again.");
                Console.WriteLine();

                return false;
            }
            else
            {
                return true;
            }
        }

        private void SetupConfigurationProviders()
        {

            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(AppSettingsFile, optional: true)
                .AddEnvironmentVariables();

            _configuration = configBuilder.Build();
        }
    }
}
