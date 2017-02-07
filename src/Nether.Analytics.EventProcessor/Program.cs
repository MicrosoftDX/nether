using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;

namespace Nether.Analytics.EventProcessor
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    static class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        public static void Main()
        {
            // Read configuration
            //TODO: Make all configuration work in the same way across Nether
            Console.WriteLine("Configuring WebJob (from Environment Variables");
            var webJobDashboardAndStorageConnectionString = 
                Environment.GetEnvironmentVariable("NETHER_WEBJOB_DASHBOARD_AND_STORAGE_CONNECTIONSTRING");
            Console.WriteLine($"webJobDashboardAndStorageConnectionString: {webJobDashboardAndStorageConnectionString}");
            var ingestEventHubConnectionString = 
                Environment.GetEnvironmentVariable("NETHER_INGEST_EVENTHUB_CONNECTIONSTRING");
            Console.WriteLine($"ingestEventHubConnectionString: {ingestEventHubConnectionString}");
            var ingestEventHubName = 
                Environment.GetEnvironmentVariable("NETHER_INGEST_EVENTHUB_NAME");
            Console.WriteLine($"ingestEventHubName: {ingestEventHubName}");
            Console.WriteLine();

            // Configure WebJob

            var jobHostConfig = new JobHostConfiguration(webJobDashboardAndStorageConnectionString);
            var eventHubConfig = new EventHubConfiguration();
            eventHubConfig.AddReceiver(ingestEventHubName, ingestEventHubConnectionString);

            jobHostConfig.UseEventHub(eventHubConfig);

            if (jobHostConfig.IsDevelopment)
            {
                jobHostConfig.UseDevelopmentSettings();
            }

            // Run and block
            var host = new JobHost(jobHostConfig);
            host.RunAndBlock();
        }
    }
}
