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
        private static Random _rnd = new Random();

        public string DefaultProperty => "rnd";

        public string GeneratePropertyValue()
        {
            return _rnd.NextDouble().ToString();
        }
    }
}
