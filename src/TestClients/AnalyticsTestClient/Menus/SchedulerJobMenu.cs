// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest.Azure.Authentication;
using Nether.Analytics;
using Nether.Analytics.DataLake;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AnalyticsTestClient
{
    public class SchedulerJobMenu : ConsoleMenu
    {
        //waiting time for the job executor loop
        private readonly TimeSpan _waitTimeSpan = TimeSpan.FromMinutes(5);

        public SchedulerJobMenu()
        {
            Title = "Run Scheduler";
            MenuItems.Add('1', new ConsoleMenuItem("Run job{1-3}", async () => { await StartJobSchedulerAsync(); }));
            MenuItems.Add('2', new ConsoleMenuItem("Stop job scheduler", CancelJobScheduler));
        }

        //a small implementation that correlates job strings with actual job files
        //just for demo/PoC purposes
        private const string job1 = "job1", job2 = "job2", job3 = "job3";
        private Dictionary<string, string> _jobToUSQLFiles = new Dictionary<string, string>()
        {
            {job1, @"C:\tmp\clusters.usql" },
            {job2, @"C:\tmp\DAU.usql" },
            {job3, @"C:\tmp\geoclustering.usql" }
        };

        private static CancellationToken s_cancellationToken;
        private static CancellationTokenSource s_cancellationTokenSource;

        private async Task StartJobSchedulerAsync()
        {
            if (s_cancellationTokenSource != null)
            {
                Console.WriteLine("Job scheduler is already running. No action performed");
                return;
            }
            s_cancellationTokenSource = new CancellationTokenSource();
            s_cancellationToken = s_cancellationTokenSource.Token;

            // Authenticate against Azure AD once and re-use for all needed purposes
            var serviceClientCredentials = await ApplicationTokenProvider.LoginSilentAsync(
                Config.Root[Config.NAH_AAD_Domain],
                new ClientCredential(Config.Root[Config.NAH_AAD_CLIENTID], Config.Root[Config.NAH_AAD_CLIENTSECRET]));

            string storageConnectionstring = Config.Root[Config.NAH_EHLISTENER_STORAGECONNECTIONSTRING];

            var stateProvider = new BlobStorageStateProvider(storageConnectionstring);  // Responsible for state
            var runDailyAtFiveAM = new RunOncePerDaySchedule(stateProvider, 5, 0, new DateTime(2017, 6, 10));
            var syncProvider = new BlobSynchronizationProvider(storageConnectionstring); // Responsible for Singleton Implementation
            var jobExecutor = new JobExecutor(syncProvider);

            await Task.Run(async () =>
            {
                while (true)
                {
                    if (s_cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine("Cancellation Requested. Stopping job scheduler");
                        return;
                    }

                    var job1Task = jobExecutor.RunAsSingletonAsync(job1, runDailyAtFiveAM, async (d) =>
                    {
                        // variable d is of type DateTime and is set to the date and time when the job
                        // was supposed to have run, i.e. on the hour of whatever hour we should have run on

                        var jobManager = new DataLakeAnalyticsJobManager(serviceClientCredentials,
                            Config.Root[Config.NAH_AZURE_SUBSCRIPTIONID], Config.Root[Config.NAH_AZURE_DLA_ACCOUNTNAME]);

                        // Set parameters and others here
                        string script = File.ReadAllText(_jobToUSQLFiles[job1]);
                        await jobManager.SubmitJobAsync(job1, script, null);
                    });

                    var job2Task = jobExecutor.RunAsSingletonAsync(job2, runDailyAtFiveAM, async (d) =>
                    {
                        var jobManager = new DataLakeAnalyticsJobManager(serviceClientCredentials,
                        Config.Root[Config.NAH_AZURE_SUBSCRIPTIONID], Config.Root[Config.NAH_AZURE_DLA_ACCOUNTNAME]);
                        string script = File.ReadAllText(_jobToUSQLFiles[job2]);
                        await jobManager.SubmitJobAsync(job2, script, null);
                    });

                    var job3Task = jobExecutor.RunAsSingletonAsync(job3, runDailyAtFiveAM, async (d) =>
                    {
                        var jobManager = new DataLakeAnalyticsJobManager(serviceClientCredentials,
                        Config.Root[Config.NAH_AZURE_SUBSCRIPTIONID], Config.Root[Config.NAH_AZURE_DLA_ACCOUNTNAME]);
                        string script = File.ReadAllText(_jobToUSQLFiles[job3]);
                        await jobManager.SubmitJobAsync(job3, script, null);
                    });

                    Task.WaitAll(job1Task, job2Task, job3Task);

                    await Task.Delay(_waitTimeSpan, s_cancellationToken);
                }
            }, s_cancellationToken);
        }

        private void CancelJobScheduler()
        {
            if (s_cancellationTokenSource != null)
            {
                s_cancellationTokenSource.Cancel();
                s_cancellationTokenSource.Dispose();
                s_cancellationTokenSource = null;
            }
            else
            {
                Console.WriteLine("Job scheduler is not running. No action performed");
            }
        }
    }
}
