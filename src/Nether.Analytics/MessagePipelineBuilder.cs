// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace Nether.Analytics
{
    public class MessagePipelineBuilder
    {
        private string _pipelineName;
        private List<string> _messageTypesToHandle = new List<string>();
        private List<IMessageHandler> _handlers = new List<IMessageHandler>();
        private IOutputManager[] _outputManagers;

        public MessagePipelineBuilder(string pipelineName)
        {
            _pipelineName = pipelineName;
        }

        public MessagePipelineBuilder HandlesMessageType(string messageType, int majorVersion, int minorVersion)
        {
            _messageTypesToHandle.Add(Message.GetVersionedMessageType(messageType, $"{majorVersion}.{minorVersion}.0"));

            return this;
        }

        public MessagePipelineBuilder AddHandler(IMessageHandler msgHandler)
        {
            _handlers.Add(msgHandler);

            return this;
        }

        public MessagePipeline Build()
        {
            return new MessagePipeline(_pipelineName, _messageTypesToHandle.ToArray(), _handlers.ToArray(), _outputManagers);
        }

        public void OutputTo(params IOutputManager[] outputManagers)
        {
            _outputManagers = outputManagers;
        }
    }
}