// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.VisualStudio.TestTools.LoadTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetherLoadTest.Helpers
{
    public static class ContextHelpers
    {
        public static TResult GetWithDefault<TResult>(this IDictionary<string, object> dictionary, string key, TResult defaultValue)
        {
            object value;
            if (dictionary.TryGetValue(key, out value))
            {
                return (TResult)value;
            }
            return defaultValue;
        }
    }
}
