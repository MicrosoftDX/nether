// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Nether.Ingest.Parsers
{
    public interface IMessageParser<T>
    {
        Task<Message> ParseMessageAsync(T msg);
    }
}