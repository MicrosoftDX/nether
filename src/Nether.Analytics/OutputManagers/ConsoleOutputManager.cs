// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    public class ConsoleOutputManager : IOutputManager
    {
        private IMessageFormatter _serializer;
        private bool _enabled;

        public ConsoleOutputManager(IMessageFormatter serializer, bool enabled = true)
        {
            _serializer = serializer;
            _enabled = enabled;
        }

        public Task OutputMessageAsync(string partitionId, string pipelineName, int index, Message msg)
        {
            if (_enabled)
            {
                var str = _serializer.Format(msg);

                Console.WriteLine(str);
            }

            return Task.CompletedTask;
        }

        public Task FlushAsync(string partitionId)
        {
            return Task.CompletedTask;
        }
    }
}
