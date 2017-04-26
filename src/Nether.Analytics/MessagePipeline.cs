// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace Nether.Analytics
{
    public class MessagePipeline<ParsedMessageType> where ParsedMessageType : IKnownMessageType
    {
        private IMessageHandler<ParsedMessageType>[] _gameEventHandlers;
        private IOutputManager<ParsedMessageType>[] _outputManagers;

        public string MessageType { get; private set; }

        public MessagePipeline(string messageType,
            IMessageHandler<ParsedMessageType>[] gameEventHandlers,
            IOutputManager<ParsedMessageType>[] outputManagers)
        {
            MessageType = messageType;
            _gameEventHandlers = gameEventHandlers;
            _outputManagers = outputManagers;
        }

        public void ProcessMessage(ParsedMessageType msg)
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