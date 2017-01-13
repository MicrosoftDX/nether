using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Nether.Web.UnitTests.Utilities
{
    public abstract class JsonConfigTestBase : IDisposable
    {
        private string _filename;
        protected IConfiguration LoadConfig(string json)
        {
            _filename = Path.GetTempFileName();

            File.WriteAllText(_filename, json);

            return new ConfigurationBuilder()
                .AddJsonFile(_filename)
                .Build();
        }
        public void Dispose()
        {
            // clean up temporary config file
            if (File.Exists(_filename))
                File.Delete(_filename);
        }
    }
}
