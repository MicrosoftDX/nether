// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.Management.DataLake.Analytics;
using Microsoft.Azure.Management.DataLake.Analytics.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

        public async Task<Guid> SubmitJobAsync(string jobName, string script, Dictionary<string, object> variables = null)
        {
            if (variables != null)
            {
                script = UsqlHelper.ReplaceVariableValuesInScript(script, variables, out var variablesFound);
            }

            Console.WriteLine(script);

            var jobId = Guid.NewGuid();
            var properties = new USqlJobProperties(script);
            var jobParameters = new JobInformation(jobName, JobType.USql, properties, priority: 1, degreeOfParallelism: 1, jobId: jobId);

            await _dlaJobClient.Job.CreateAsync("nether", jobId, jobParameters);

            return jobId;
        }

        public JobInformation WaitForJob(Guid jobId)
        {
            var jobInfo = _dlaJobClient.Job.Get(_dlaAccountName, jobId);
            while (jobInfo.State != JobState.Ended)
            {
                jobInfo = _dlaJobClient.Job.Get(_dlaAccountName, jobId);
                Console.WriteLine(jobInfo.State);
            }
            return jobInfo;
        }
    }
}
