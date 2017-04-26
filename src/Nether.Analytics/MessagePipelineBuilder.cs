// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace Nether.Analytics
{
    public class MessagePipelineBuilder<ParsedMessageType> where ParsedMessageType : IKnownMessageType
    {
        private string _eventName;
        private List<IMessageHandler<ParsedMessageType>> _handlers = new List<IMessageHandler<ParsedMessageType>>();
        private IOutputManager<ParsedMessageType>[] _outputManagers;

        public MessagePipelineBuilder(string eventName)
        {
            _eventName = eventName;
        }

        public MessagePipelineBuilder<ParsedMessageType> AddHandler(IMessageHandler<ParsedMessageType> eventHandler)
        {
            _handlers.Add(eventHandler);

            return this;
        }

        public MessagePipeline<ParsedMessageType> Build(List<IMessageHandler<ParsedMessageType>> globalHandlers)
        {
            return new MessagePipeline<ParsedMessageType>(_eventName, globalHandlers.Concat(_handlers).ToArray(), _outputManagers);
        }

        public void OutputTo(params IOutputManager<ParsedMessageType>[] outputManagers)
        {
            _outputManagers = outputManagers;
        }
    }
}