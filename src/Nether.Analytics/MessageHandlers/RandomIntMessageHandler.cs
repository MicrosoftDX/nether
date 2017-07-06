// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Nether.Ingest
{
    public class RandomIntMessageHandler : ValueMessageHandler
    {
        public RandomIntMessageHandler(params string[] properties) : base(new RandomIntValueGenerator(), properties)
        {
        }

        public RandomIntMessageHandler(int maxValue, params string[] properties) : base(new RandomIntValueGenerator(maxValue: maxValue), properties)
        {
        }

        public RandomIntMessageHandler(int minValue, int maxValue, params string[] properties) : base(new RandomIntValueGenerator(minValue: minValue, maxValue: maxValue), properties)
        {
        }
    }

    public class RandomIntValueGenerator : IValueGenerator
    {
        private static Random s_rnd = new Random();

        private int _minValue;
        private int _maxValue;

        public RandomIntValueGenerator(int minValue = 0, int maxValue = int.MaxValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
        }

        public string DefaultProperty => "rnd";

        public string GeneratePropertyValue()
        {
            return s_rnd.Next(_minValue, _maxValue).ToString();
        }
    }
}
