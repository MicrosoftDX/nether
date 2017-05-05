// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


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

        public void ProcessMessage(IMessage msg)
        {
            foreach (var handler in _gameEventHandlers)
            {
                var result = handler.ProcessMessage(msg);
                if (result == MessageHandlerResluts.FailStopProcessing)
                {
                    return;
                }
            }

            foreach (var outputManager in _outputManagers)
            {
                outputManager.OutputMessageAsync(msg);
            }
        }
    }
}