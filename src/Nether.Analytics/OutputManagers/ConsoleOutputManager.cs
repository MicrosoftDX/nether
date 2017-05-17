// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    public class ConsoleOutputManager : IOutputManager
    {
        private IOutputFormatter _serializer;

        public ConsoleOutputManager(IOutputFormatter serializer)
        {
            _serializer = serializer;
        }

        public Task FlushAsync()
        {
            return Task.CompletedTask;
        }

        public Task OutputMessageAsync(string pipelineName, int idx, Message msg)
        {
            var str = _serializer.Format(msg);

            Console.WriteLine(str);

            return Task.CompletedTask;
        }
    }
}
