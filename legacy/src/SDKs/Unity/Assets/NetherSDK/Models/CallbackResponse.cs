// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetherSDK.Models
{
    /// <summary>
    /// Holds the response status and possible exception instance
    /// </summary>
    public class CallbackResponse
    {
        public CallBackResult Status { get; set; }
        public Exception Exception { get; set; }
        public NetherErrorResult NetherError { get; set; }
    }

    /// <summary>
    /// Holds a Result object with a V value
    /// </summary>
    /// <typeparam name="V">The Result object of the callback</typeparam>
    public class CallbackResponse<V> : CallbackResponse
        where V : class
    {
        public V Result { get; set; }
    }

    public enum CallBackResult
    {
        Success,
        Failure,
        DeserializationFailure,
        NotFound,
        LocalException,
        ResourceExists,
        Unauthorized
    }
}