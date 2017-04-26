// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.Management.DataLake.Store;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;
using System.Threading.Tasks;
using System;

namespace Nether.Analytics.DataLake
{
    public class DataLakeStoreOutputManager : DataLakeStoreOutputManager<Message>
    {
        public DataLakeStoreOutputManager(ServiceClientCredentials serviceClientCredentials, string adlsAccountName) : base(serviceClientCredentials, adlsAccountName)
        {
        }

        public DataLakeStoreOutputManager(string domain, ClientCredential clientCredential, string subscriptionId, string adlsAccountName) : base(domain, clientCredential, subscriptionId, adlsAccountName)
        {
        }

        public DataLakeStoreOutputManager(string domain, string clientId, string clientSecret, string subscriptionId, string adlsAccountName) : base(domain, clientId, clientSecret, subscriptionId, adlsAccountName)
        {
        }
    }

    public class DataLakeStoreOutputManager<T> : IOutputManager<T>
    {
        private ClientCredential _clientCredential;
        private string _domain;
        private string _subscriptionId;
        private string _adlsAccountName;

        //private DataLakeStoreAccountManagementClient _adlsClient;
        private DataLakeStoreFileSystemManagementClient _adlsFileSystemClient;
        private ServiceClientCredentials _serviceClientCredentials;

        public bool IsAuthenticated
        {
            get
            {
                return _serviceClientCredentials != null;
            }
        }

        public DataLakeStoreOutputManager(string domain, string clientId, string clientSecret, string subscriptionId, string adlsAccountName)
            : this(domain, new ClientCredential(clientId, clientSecret), subscriptionId, adlsAccountName)
        {
        }

        public DataLakeStoreOutputManager(string domain, ClientCredential clientCredential, string subscriptionId, string adlsAccountName)
        {
            _domain = domain;
            _clientCredential = clientCredential;
            _subscriptionId = subscriptionId;
            _adlsAccountName = adlsAccountName;
        }

        public DataLakeStoreOutputManager(ServiceClientCredentials serviceClientCredentials, string adlsAccountName)
        {
            _serviceClientCredentials = serviceClientCredentials;
            _adlsAccountName = adlsAccountName;

            SetupClient();
        }

        private async Task Authenticate()
        {
            _serviceClientCredentials = await ApplicationTokenProvider.LoginSilentAsync(_domain, _clientCredential);
        }

        private void SetupClient()
        {
            _adlsFileSystemClient = new DataLakeStoreFileSystemManagementClient(_serviceClientCredentials);
        }

        public async Task OutputMessageAsync(T msg)
        {
            if (!IsAuthenticated)
            {
                await Authenticate();
                SetupClient();
            }

            var result = await _adlsFileSystemClient.FileSystem.MkdirsAsync(_adlsAccountName, "test");

            Console.WriteLine(result);
        }

        public Task Flush()
        {
            return Task.CompletedTask;
        }
    }
}
