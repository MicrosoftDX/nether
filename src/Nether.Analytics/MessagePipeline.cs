// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System.Threading.Tasks;

namespace Nether.Analytics
{
    public class MessagePipeline
    {
        private IMessageHandler[] _gameEventHandlers;
        private IOutputManager[] _outputManagers;

        public string MessageType { get; private set; }

        public MessagePipeline(string messageType,
            IMessageHandler[] gameEventHandlers,
            IOutputManager[] outputManagers)
        {
            MessageType = messageType;
            _gameEventHandlers = gameEventHandlers;
            _outputManagers = outputManagers;
        }

        public async Task ProcessMessageAsync(IMessage msg)
        {
            foreach (var handler in _gameEventHandlers)
            {
                var result = await handler.ProcessMessageAsync(msg);
                if (result == MessageHandlerResluts.FailStopProcessing)
                {
                    //TODO: Implement better solution to breaking out of chain of processing messages
                    return;
                }
            }

            //TODO: Run output managers in parallel
            foreach (var outputManager in _outputManagers)
            {
                await outputManager.OutputMessageAsync(msg);
            }
        }
    }
}