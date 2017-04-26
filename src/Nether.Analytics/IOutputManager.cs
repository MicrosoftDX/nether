// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    public interface IOutputManager<T>
    {
        /// <summary>
        /// Outputs a message to the correct output. Depending on the implementation, outputs might
        /// be batched together and written later.
        /// </summary>
        /// <param name="msg">The message to be written</param>
        /// <returns></returns>
        Task OutputMessageAsync(T msg);
        /// <summary>
        /// Flushes andy unwritten messages to the correct output
        /// </summary>
        /// <returns></returns>
        Task Flush();
    }
}