// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.



using System;
using Nether.Analytics.Parsers;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    public class EventHubOutputManager : IOutputManager
    {
        private string _outputEventHubConnectionString;

        public EventHubOutputManager(string outputEventHubConnectionString)
        {
            _outputEventHubConnectionString = outputEventHubConnectionString;
        }

        public Task Flush()
        {
            throw new NotImplementedException();
        }

        public Task OutputMessageAsync(IMessage msg)
        {
            throw new NotImplementedException();
        }
    }
}