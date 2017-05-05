// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace Nether.Analytics
{
    public class MessagePipelineBuilder
    {
        private string _eventName;
        private List<IMessageHandler> _handlers = new List<IMessageHandler>();
        private IOutputManager[] _outputManagers;

        public MessagePipelineBuilder(string eventName)
        {
            _eventName = eventName;
        }

        public MessagePipelineBuilder AddHandler(IMessageHandler msgHandler)
        {
            _handlers.Add(msgHandler);

            return this;
        }

        public MessagePipeline Build(List<IMessageHandler> globalMsgHandlers)
        {
            return new MessagePipeline(_eventName, globalMsgHandlers.Concat(_handlers).ToArray(), _outputManagers);
        }

        public void OutputTo(params IOutputManager[] outputManagers)
        {
            _outputManagers = outputManagers;
        }
    }
}