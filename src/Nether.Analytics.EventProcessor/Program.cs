using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace Nether.Analytics.EventProcessor
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    static class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        public static void Main()
        {
            // Configure WebJob
            var jobHostConfig = new JobHostConfiguration("");
            var eventHubConfig = new EventHubConfiguration();
            eventHubConfig.AddReceiver("analytics", "");
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
