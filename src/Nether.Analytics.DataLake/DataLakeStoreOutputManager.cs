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

namespace Nether.Analytics.DataLake
{
    public class DataLakeStoreOutputManager : IOutputManager
    {
        private IMessageSerializer _serializer;
        //private IFilePathAlgoritm _filePathAlgoritm;
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

        public DataLakeStoreOutputManager(IMessageSerializer serializer, string domain, string clientId, string clientSecret, string subscriptionId, string adlsAccountName)
            : this(serializer, domain, new ClientCredential(clientId, clientSecret), subscriptionId, adlsAccountName)
        {
        }

        public DataLakeStoreOutputManager(IMessageSerializer serializer, string domain, ClientCredential clientCredential, string subscriptionId, string adlsAccountName)
        {
            _serializer = serializer;

            _domain = domain;
            _clientCredential = clientCredential;

            _subscriptionId = subscriptionId;
            _adlsAccountName = adlsAccountName;
        }

        public DataLakeStoreOutputManager(IMessageSerializer serializer, ServiceClientCredentials serviceClientCredentials, string subscriptionId, string adlsAccountName)
        {
            _serializer = serializer;

            _serviceClientCredentials = serviceClientCredentials;

            _subscriptionId = subscriptionId;
            _adlsAccountName = adlsAccountName;

            _adlsFileSystemClient = new DataLakeStoreFileSystemManagementClient(_serviceClientCredentials);
        }


        public async Task OutputMessageAsync(IMessage msg)
        {
            await CheckAuthentication();

            var filePath = GetFilePath(msg);
            var content = _serializer.Serialize(msg);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                await _adlsFileSystemClient.FileSystem.ConcurrentAppendAsync(_adlsAccountName, filePath, stream, AppendModeType.Autocreate);
            }
        }

        public Task Flush()
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

        private string GetFilePath(IMessage msg)
        {
            var t = msg.EnqueueTimeUtc;

            var path = $"/nether/{msg.MessageType}/{t.Year:D4}/{t.Month:D2}/{t.Day:D2}/";
            var fileName = $"{t.Hour:D2}_{t.Minute:D2}";
            var fileExtension = ".csv";

            return path + fileName + fileExtension;
        }
    }

    //public interface IFilePathAlgoritm
    //{
    //    string GetPath(IMessage msg);
    //}

    //public interface IFileNameAlgoritm
    //{
    //    string GetName(IMessage msg);
    //}
}
