// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Nether.Analytics.Host
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine(@"   _   _      _   _               ");
            Console.WriteLine(@"  | \ | | ___| |_| |__   ___ _ __ ");
            Console.WriteLine(@"  |  \| |/ _ \ __| '_ \ / _ \ '__|");
            Console.WriteLine(@"  | |\  |  __/ |_| | | |  __/ |   ");
            Console.WriteLine(@"  |_| \_|\___|\__|_| |_|\___|_| Analytics");
            Console.WriteLine(@"  Message Processor Host ");
            Console.WriteLine();

            var app = new ProgramEx();

            Task.Run(async () =>
            {
                await app.RunAsync();
            }).GetAwaiter().GetResult();
        }
    }
}
