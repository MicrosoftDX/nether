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
        private IOutputFormatter _serializer;
        private string _ehConnectionString;

        /// <summary>
        /// Creates a new instance, with a JSON output formatter as the default serializer.
        /// </summary>
        /// <param name="outputEventHubConnectionString">The connection string for the event hub output.</param>
        public EventHubOutputManager(string outputEventHubConnectionString) : this(outputEventHubConnectionString, new JsonOutputFormatter())
        {
        }

        public EventHubOutputManager(string outputEventHubConnectionString, IOutputFormatter serializer)
        {
            _serializer = serializer;
            _ehConnectionString = outputEventHubConnectionString;
        }

        public Task FlushAsync()
        {
            throw new NotImplementedException();
        }

        public Task OutputMessageAsync(string pipelineName, int idx, Message msg)
        {
            throw new NotImplementedException();
        }
    }
}