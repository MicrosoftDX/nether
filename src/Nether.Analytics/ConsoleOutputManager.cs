// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    public class ConsoleOutputManager : ConsoleOutputManager<Message>
    {
        public ConsoleOutputManager(IMessageSerializer<Message> serializer) : base(serializer)
        {
        }
    }

    public class ConsoleOutputManager<T> : IOutputManager<T>
    {
        private IMessageSerializer<T> _serializer;

        public ConsoleOutputManager(IMessageSerializer<T> serializer)
        {
            _serializer = serializer;
        }

        public Task Flush()
        {
            return Task.CompletedTask;
        }

        public Task OutputMessageAsync(T msg)
        {
            var str = _serializer.Serialize(msg);

            Console.WriteLine(str);

            return Task.CompletedTask;
        }
    }
}
