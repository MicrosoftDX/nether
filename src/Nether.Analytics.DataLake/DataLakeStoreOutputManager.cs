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
        public DataLakeStoreOutputManager(IMessageSerializer serializer, ServiceClientCredentials serviceClientCredentials, string subscriptionId, string adlsAccountName) : base(serializer, serviceClientCredentials, subscriptionId, adlsAccountName)
        {
        }

        public DataLakeStoreOutputManager(IMessageSerializer serializer, string domain, ClientCredential clientCredential, string subscriptionId, string adlsAccountName) : base(serializer, domain, clientCredential, subscriptionId, adlsAccountName)
        {
        }

        public DataLakeStoreOutputManager(IMessageSerializer serializer, string domain, string clientId, string clientSecret, string subscriptionId, string adlsAccountName) : base(serializer, domain, clientId, clientSecret, subscriptionId, adlsAccountName)
        {
        }
    }

    public class DataLakeStoreOutputManager<T> : IOutputManager<T>
    {
        private IMessageSerializer<T> _serializer;
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

        public DataLakeStoreOutputManager(IMessageSerializer<T> serializer, string domain, string clientId, string clientSecret, string subscriptionId, string adlsAccountName)
            : this(serializer, domain, new ClientCredential(clientId, clientSecret), subscriptionId, adlsAccountName)
        {
        }

        public DataLakeStoreOutputManager(IMessageSerializer<T> serializer, string domain, ClientCredential clientCredential, string subscriptionId, string adlsAccountName)
        {
            _serializer = serializer;

            _domain = domain;
            _clientCredential = clientCredential;

            _subscriptionId = subscriptionId;
            _adlsAccountName = adlsAccountName;
        }

        public DataLakeStoreOutputManager(IMessageSerializer<T> serializer, ServiceClientCredentials serviceClientCredentials, string subscriptionId, string adlsAccountName)
        {
            _serializer = serializer;

            _serviceClientCredentials = serviceClientCredentials;

            _subscriptionId = subscriptionId;
            _adlsAccountName = adlsAccountName;

            _adlsFileSystemClient = new DataLakeStoreFileSystemManagementClient(_serviceClientCredentials);
        }

        private async Task AuthenticateAsync()
        {
            _serviceClientCredentials = await ApplicationTokenProvider.LoginSilentAsync(_domain, _clientCredential);
        }

        public async Task OutputMessageAsync(T msg)
        {
            if (!IsAuthenticated)
            {
                await AuthenticateAsync();
                _adlsFileSystemClient = new DataLakeStoreFileSystemManagementClient(_serviceClientCredentials);
            }

            _adlsFileSystemClient.FileSystem.Mkdirs(_adlsAccountName, "/nutestarvi");

            var result = await _adlsFileSystemClient.FileSystem.MkdirsAsync(_adlsAccountName, "/test");

            Console.WriteLine(result);
        }

        public Task Flush()
        {
            return Task.CompletedTask;
        }
    }
}
