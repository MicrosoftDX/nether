// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest.Azure.Authentication;
using Nether.BlobStorage;
using Nether.DataLake;
using Nether.Ingest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Nether.Test.ConsoleClient
{
    public class SchedulerJobMenu : ConsoleMenu
    {
        //waiting time for the job executor loop
        private readonly TimeSpan _waitTimeSpan = TimeSpan.FromMinutes(5);
        private static CancellationToken s_cancellationToken;
        private static CancellationTokenSource s_cancellationTokenSource;
        private const string job1 = "job1", job2 = "job2", job3 = "job3";
        //a small implementation that correlates job strings with actual job files
        //just for demo/PoC purposes
        private Dictionary<string, string> _jobToUSQLFiles = new Dictionary<string, string>()
        {
            {job1, @"C:\tmp\clusters.usql" },
            {job2, @"C:\tmp\DAU.usql" },
            {job3, @"C:\tmp\geoclustering.usql" }
        };

        public SchedulerJobMenu()
        {
            Title = "Run Scheduler";
            MenuItems.Add('1', new ConsoleMenuItem("Run simple job every 5 seconds", RunSimpleJob));
            MenuItems.Add('2', new ConsoleMenuItem("Run long job minute on the minute", RunLongJob));
            MenuItems.Add('3', new ConsoleMenuItem("Remove job state", ResetSimpleJobState));
            MenuItems.Add('4', new ConsoleMenuItem("Run job{1-3}", async () => { await StartJobSchedulerAsync(); }));
            MenuItems.Add('5', new ConsoleMenuItem("Stop job scheduler", CancelJobScheduler));
        }

        private void RunSimpleJob()
        {
            var startTime = ConsoleEx.ReadLine("Start Time (UTC)", DateTime.UtcNow - TimeSpan.FromMinutes(1));
            var jobId = ConsoleEx.ReadLine("JobId", "simpleJob001");

            var storageConnectionstring = Config.Root[Config.NAH_EHLISTENER_STORAGECONNECTIONSTRING];

            var syncProvider = new BlobSynchronizationProvider(storageConnectionstring); // Responsible for Singleton Implementation
            var stateProvider = new BlobJobStateProvider(storageConnectionstring);  // Responsible for state
            var jobExecutor = new JobExecutor(syncProvider, stateProvider);
            var every5Seconds = new IntervalSchedule(TimeSpan.FromSeconds(5));

            while (true)
            {
                jobExecutor.RunAsSingletonAsync(jobId, every5Seconds, (dt) =>
                {
                    Console.WriteLine($"Start Job for Time {dt}");
                    return Task.CompletedTask;
                }, startTime).Wait();

                Console.Write(".");

                Thread.Sleep(1000);

                if (EscPressed())
                    break;
            }
        }

        private void RunLongJob()
        {
            var startTime = ConsoleEx.ReadLine("Start Time (UTC)", DateTime.UtcNow - TimeSpan.FromMinutes(1));
            var jobId = ConsoleEx.ReadLine("JobId", "longJob001");

            var storageConnectionstring = Config.Root[Config.NAH_EHLISTENER_STORAGECONNECTIONSTRING];

            var syncProvider = new BlobSynchronizationProvider(storageConnectionstring); // Responsible for Singleton Implementation
            var stateProvider = new BlobJobStateProvider(storageConnectionstring);  // Responsible for state
            var jobExecutor = new JobExecutor(syncProvider, stateProvider);
            var every5Seconds = new EveryMinuteOnTheMinuteSchedule();

            while (true)
            {
                jobExecutor.RunAsSingletonAsync(jobId, every5Seconds, (dt) =>
                {
                    Console.WriteLine($"Start 15s Job for Time {dt}");
                    Thread.Sleep(TimeSpan.FromSeconds(15));
                    Console.WriteLine("Job done!");

                    return Task.CompletedTask;
                }, startTime).Wait();

                Console.Write(".");

                Thread.Sleep(1000);

                if (EscPressed())
                    break;
            }
        }

        private void ResetSimpleJobState()
        {
            var storageConnectionstring = Config.Root[Config.NAH_EHLISTENER_STORAGECONNECTIONSTRING];

            var stateProvider = new BlobJobStateProvider(storageConnectionstring);

            string jobId = ConsoleEx.ReadLine("JobId", "simpleJob001");

            try
            {
                stateProvider.DeleteEntryAsync(jobId).Wait();
                Console.WriteLine("Job state removed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to remove job state for job {jobId}");
                Console.WriteLine(ex.Message);
            }
        }

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

            var storageConnectionstring = Config.Root[Config.NAH_EHLISTENER_STORAGECONNECTIONSTRING];

            var stateProvider = new BlobJobStateProvider(storageConnectionstring);  // Responsible for state
            var runDailyAtFiveAM = new DailySchedule(05, 00, 00);
            var syncProvider = new BlobSynchronizationProvider(storageConnectionstring); // Responsible for Singleton Implementation
            var jobExecutor = new JobExecutor(syncProvider, stateProvider);

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
