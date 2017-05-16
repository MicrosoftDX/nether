using System;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    public class ConsoleCorruptMessageHandler : ICorruptMessageHandler
    {
        public Task HandleAsync(string msg)
        {
            return Task.Run(() => Console.WriteLine($"Dumping corrupt message {msg}"));
        }
    }
}
