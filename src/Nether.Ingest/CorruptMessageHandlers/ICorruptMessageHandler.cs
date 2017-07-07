// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Nether.Ingest
{
    public interface ICorruptMessageHandler
    {
        Task HandleAsync(string msg);
    }
}
