// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    public class ConsoleCorruptMessageHandler : ICorruptMessageHandler
    {
        public Task HandleAsync(string msg)
        {
            return Task.Run(() => Console.WriteLine($"Dumping corrupt message {msg}"));
        }
    }
}
