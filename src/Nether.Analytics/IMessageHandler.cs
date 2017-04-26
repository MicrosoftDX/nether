// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// KEEP

namespace Nether.Analytics
{
    public interface IMessageHandler : IMessageHandler<Message>
    {
    }

    public interface IMessageHandler<T> where T : IKnownMessageType
    {
        MessageHandlerResluts ProcessMessage(T message);
    }
}