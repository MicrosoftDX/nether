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
