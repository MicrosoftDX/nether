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
                script = ReplaceUSqlVariableValues(script, variables);
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

        private static string ReplaceUSqlVariableValues(string script, params Tuple<string, object>[] variableValuePairs)
        {
            return ReplaceUSqlVariableValues(script, variableValuePairs.ToDictionary(t => t.Item1, t => t.Item2));
        }

        private static string ReplaceUSqlVariableValues(string script, Dictionary<string, object> variables)
        {
            var variablesToAdd = new List<KeyValuePair<string, object>>();

            foreach (var variable in variables)
            {
                script = ReplaceUsqlVariableValue(script, variable.Key, variable.Value, out var variableFound);
                if (!variableFound)
                {
                    variablesToAdd.Add(variable);
                }
            }

            return script;
        }

        private static string ReplaceUsqlVariableValue(string script, string variable, object value, out bool variableFound)
        {
            //TODO: Find better regex that allows string variables in just one replace statement
            // Current implementation doesn't isolate content of quoted values, therefor the extra
            // check to see if value is string or not

            var pattern = @"(?<=DECLARE\s*" + variable + @"((\s*)|(\s.*))=\s*)([\S].*)(?=\s*;)";

            var matches = Regex.Matches(script, pattern, RegexOptions.Multiline);
            if (matches.Count == 0)
            {
                variableFound = false;
                return script;
            }
            else if (matches.Count == 1)
            {
                variableFound = true;

                if (value is string)
                {
                    return Regex.Replace(script, pattern, "\"" + value + "\"");
                }
                else
                {
                    return Regex.Replace(script, pattern, value.ToString());
                }
            }
            else
            {
                throw new Exception($"Variable {variable} was found in more than one place in the script");
            }
        }

    }
}
