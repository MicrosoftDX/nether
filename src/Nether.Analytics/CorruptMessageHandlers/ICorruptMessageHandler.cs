using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    public interface ICorruptMessageHandler
    {
        Task HandleAsync(string msg);
    }
}
