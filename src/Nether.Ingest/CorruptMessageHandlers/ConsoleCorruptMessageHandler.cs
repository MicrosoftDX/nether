// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Nether.Ingest
{
    public class ConsoleCorruptMessageHandler : ICorruptMessageHandler
    {
        public Task HandleAsync(string msg)
        {
            Console.WriteLine($"Dumping corrupt message {msg}");
            return Task.CompletedTask;
        }
    }
}
