// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.Management.DataLake.Store;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Text;
using Microsoft.Azure.Management.DataLake.Store.Models;
using System.Linq;

namespace Nether.Analytics.DataLake
{
    public class DataLakeStoreOutputManager : IOutputManager
    {
        private IMessageSerializer _serializer;
        private IFilePathAlgorithm _filePathAlgorithm;
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

        public DataLakeStoreOutputManager(IMessageSerializer serializer, IFilePathAlgorithm filePathAlgorithm, string domain, string clientId, string clientSecret, string subscriptionId, string adlsAccountName)
            : this(serializer, filePathAlgorithm, domain, new ClientCredential(clientId, clientSecret), subscriptionId, adlsAccountName)
        {
        }

        public DataLakeStoreOutputManager(IMessageSerializer serializer, IFilePathAlgorithm filePathAlgorithm, string domain, ClientCredential clientCredential, string subscriptionId, string adlsAccountName)
        {
            _serializer = serializer;
            _filePathAlgorithm = filePathAlgorithm;

            _domain = domain;
            _clientCredential = clientCredential;

            _subscriptionId = subscriptionId;
            _adlsAccountName = adlsAccountName;
        }

        public DataLakeStoreOutputManager(IMessageSerializer serializer, IFilePathAlgorithm filePathAlgorithm, ServiceClientCredentials serviceClientCredentials, string subscriptionId, string adlsAccountName)
        {
            _serializer = serializer;
            _filePathAlgorithm = filePathAlgorithm;

            _serviceClientCredentials = serviceClientCredentials;

            _subscriptionId = subscriptionId;
            _adlsAccountName = adlsAccountName;

            _adlsFileSystemClient = new DataLakeStoreFileSystemManagementClient(_serviceClientCredentials);
        }


        public async Task OutputMessageAsync(string pipelineName, int idx, Message msg)
        {
            await CheckAuthentication();

            var filePath = GetFilePath(pipelineName, idx, msg);
            var content = _serializer.Serialize(msg);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                await _adlsFileSystemClient.FileSystem.ConcurrentAppendAsync(_adlsAccountName, filePath, stream, AppendModeType.Autocreate);
            }
        }

        public Task FlushAsync()
        {
            return Task.CompletedTask;
        }

        private async Task CheckAuthentication()
        {
            if (!IsAuthenticated)
            {
                _serviceClientCredentials = await ApplicationTokenProvider.LoginSilentAsync(_domain, _clientCredential);
                _adlsFileSystemClient = new DataLakeStoreFileSystemManagementClient(_serviceClientCredentials);
            }
        }

        private string GetFilePath(string pipelineName, int idx, Message msg)
        {
            var fp = _filePathAlgorithm.GetFilePath(pipelineName, idx, msg);

            var path = "/" + string.Join("/", fp.hierarchy) + "/";
            var fileName = $"{fp.name}.{_serializer.FileExtension}";

            return path + fileName;
        }
    }

}
