// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Nether.Analytics
{
    public class RandomDoubleMessageHandler : ValueMessageHandler
    {
        public RandomDoubleMessageHandler(params string[] properties) : base(new RandomDoubleValueGenerator(), properties)
        {
        }
    }

    public class RandomDoubleValueGenerator : IValueGenerator
    {
        private static Random s_rnd = new Random();

        public string DefaultProperty => "rnd";

        public string GeneratePropertyValue()
        {
            return s_rnd.NextDouble().ToString();
        }
    }
}
