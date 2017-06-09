// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    public class DebugMessageHandler : IMessageHandler
    {
        public Task<MessageHandlerResults> ProcessMessageAsync(Message msg, string pipelineName, int index)
        {
            Console.WriteLine();
            Console.WriteLine("DebugMessageHandler");
            Console.WriteLine("-------------------");
            Console.WriteLine($"Pipeline Name:  {pipelineName}");
            Console.WriteLine($"Handler Idx:    {index}");
            Console.WriteLine(msg.ToString());
            Console.WriteLine();

            return Task.FromResult(MessageHandlerResults.Success);
        }
    }
}