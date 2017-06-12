// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.Management.DataLake.Analytics;
using Microsoft.Azure.Management.DataLake.Analytics.Models;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nether.Analytics.DataLake
{
    public class DataLakeAnalyticsJobManager
    {
        private string _subscriptionId;
        private string _dlaAccountName;

        private DataLakeAnalyticsJobManagementClient _dlaJobClient;
        private ServiceClientCredentials _serviceClientCredentials;

        public bool IsAuthenticated
        {
            get
            {
                return _serviceClientCredentials != null;
            }
        }

        public DataLakeAnalyticsJobManager(ServiceClientCredentials serviceClientCredentials, string subscriptionId, string dlaAccountName)
        {
            _serviceClientCredentials = serviceClientCredentials;

            _subscriptionId = subscriptionId;
            _dlaAccountName = dlaAccountName;

            _dlaJobClient = new DataLakeAnalyticsJobManagementClient(serviceClientCredentials);
        }

        public async Task<Guid> SubmitJobAsync(string jobName, string script, Dictionary<string, object> variables = null, bool showScriptOnOutput = false)
        {
            if (variables != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var kvp in variables)
                {
                    sb.AppendLine($"DECLARE @{kvp.Key} = \"{kvp.Value}\";");
                }
                sb.Append(script);
                script = sb.ToString();
            }

            if (showScriptOnOutput)
                Console.WriteLine(script);

            var jobId = Guid.NewGuid();
            var properties = new USqlJobProperties(script);
            var jobParameters = new JobInformation(jobName, JobType.USql, properties, priority: 1, degreeOfParallelism: 1, jobId: jobId);

            await _dlaJobClient.Job.CreateAsync(_dlaAccountName, jobId, jobParameters);

            return jobId;
        }

        public async Task<JobInformation> WaitForJobAsync(Guid jobId, int pollingTimeout = 1000)
        {
            var jobInfo = _dlaJobClient.Job.Get(_dlaAccountName, jobId);
            while (jobInfo.State != JobState.Ended)
            {
                jobInfo = _dlaJobClient.Job.Get(_dlaAccountName, jobId);
                Console.WriteLine($"Job with Id:{jobId} currently has state {jobInfo.State}");
                await Task.Delay(pollingTimeout);
            }
            return jobInfo;
        }
    }
}
