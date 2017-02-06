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
            var dashboardAndstorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=netherdashboard;AccountKey=oT30a8/BSwTFg/4GGWLPCeGIHBfgDcMf9zEThKHlY4hjUNy3sYUTSWXWa3yJMoX2lvTnWSIrjtwU9kg9YaL0Qw==";
            var eventHubConnectionString = "Endpoint=sb://nether.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=h16jv6nc0bVfgdWrZp7f5Fpau4jaSu2YH+U2xg0YI14=";
            var eventHubName = "incomming";

            // Configure WebJob
<<<<<<< HEAD
            var jobHostConfig = new JobHostConfiguration("");
            var eventHubConfig = new EventHubConfiguration();
            eventHubConfig.AddReceiver("analytics", "");
=======
            var jobHostConfig = new JobHostConfiguration(dashboardAndstorageConnectionString);
            var eventHubConfig = new EventHubConfiguration();
            eventHubConfig.AddReceiver(eventHubName, eventHubConnectionString);
>>>>>>> 82d0142... Updates to Analytics Test Client
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
