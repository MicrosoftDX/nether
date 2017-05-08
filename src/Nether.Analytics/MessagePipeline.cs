// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System.Threading.Tasks;

namespace Nether.Analytics
{
    public class MessagePipeline
    {
        private IMessageHandler[] _gameEventHandlers;
        private IOutputManager[] _outputManagers;

        public string PipelineName { get; private set; }
        public VersionedMessageType[] HandledMessageTypes { get; private set; }

        public MessagePipeline(string pipelineName,
            VersionedMessageType[] handledMessageTypes,
            IMessageHandler[] gameEventHandlers,
            IOutputManager[] outputManagers)
        {
            PipelineName = pipelineName;
            HandledMessageTypes = handledMessageTypes;
            _gameEventHandlers = gameEventHandlers;
            _outputManagers = outputManagers;
        }

        public async Task ProcessMessageAsync(Message msg)
        {
            var handlerIdx = 0;
            foreach (var handler in _gameEventHandlers)
            {
                var result = await handler.ProcessMessageAsync(PipelineName, handlerIdx++, msg);
                if (result == MessageHandlerResluts.FailStopProcessing)
                {
                    //TODO: Implement better solution to breaking out of chain of processing messages
                    return;
                }
            }

            //TODO: Run output managers in parallel
            var outputManagerIdx = 0;
            foreach (var outputManager in _outputManagers)
            {
                await outputManager.OutputMessageAsync(PipelineName, outputManagerIdx++, msg);
            }
        }
    }
}