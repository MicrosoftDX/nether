using System;

namespace Nether.Analytics
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
        private static Random _rnd = new Random();

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
            return _rnd.Next(_minValue, _maxValue).ToString();
        }
    }
}
