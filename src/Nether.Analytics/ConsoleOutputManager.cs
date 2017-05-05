// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    public class ConsoleOutputManager : IOutputManager
    {
        private IMessageSerializer _serializer;

        public ConsoleOutputManager(IMessageSerializer serializer)
        {
            _serializer = serializer;
        }

        public Task Flush()
        {
            return Task.CompletedTask;
        }

        public Task OutputMessageAsync(IMessage msg)
        {
            var str = _serializer.Serialize(msg);

            Console.WriteLine(str);

            return Task.CompletedTask;
        }
    }
}
