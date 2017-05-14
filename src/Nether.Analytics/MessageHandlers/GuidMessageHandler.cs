// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Nether.Analytics
{
    public class GuidMessageHandler : ValueMessageHandler
    {
        public GuidMessageHandler(params string[] properties) : base(new GuidValueGenerator(), properties)
        {
        }
    }

    public class GuidValueGenerator : IValueGenerator
    {
        public string DefaultProperty => "guid";

        public string GeneratePropertyValue()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
