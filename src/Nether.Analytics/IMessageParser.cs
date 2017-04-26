// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// KEEP

using System.Threading.Tasks;

namespace Nether.Analytics.Parsers
{
    public interface IMessageParser<T> : IMessageParser<T, Message>
    {
    }

    public interface IMessageParser<TRaw, TParsed>
    {
        Task<TParsed> ParseAsync(TRaw message);
    }
}