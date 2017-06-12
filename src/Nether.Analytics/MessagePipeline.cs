// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    public class MessagePipeline
    {
        private IMessageHandler[] _gameEventHandlers;
        private IOutputManager[] _outputManagers;

        public string PipelineName { get; private set; }
        public string[] HandledMessageTypes { get; private set; }

        public MessagePipeline(string pipelineName,
            string[] handledMessageTypes,
            IMessageHandler[] gameEventHandlers,
            IOutputManager[] outputManagers)
        {
            PipelineName = pipelineName;
            HandledMessageTypes = handledMessageTypes;
            _gameEventHandlers = gameEventHandlers;
            _outputManagers = outputManagers;
        }

        public async Task ProcessMessageAsync(string partitionId, Message msg)
        {
            var handlerIdx = 0;
            foreach (var handler in _gameEventHandlers)
            {
                var result = await handler.ProcessMessageAsync(msg, PipelineName, handlerIdx++);
                if (result == MessageHandlerResults.FailStopProcessing)
                {
                    //TODO: do something better here
                    return;
                }
            }


            //TODO: Run output managers in parallel
            var outputManagerIndex = 0;
            foreach (var outputManager in _outputManagers)
            {
                await outputManager.OutputMessageAsync(partitionId, PipelineName, outputManagerIndex++, msg);
            }
        }

        public async Task FlushAsync(string partitionId)
        {
            var tasks = _outputManagers.Select(m => m.FlushAsync(partitionId));
            await Task.WhenAll(tasks);
        }
    }
}