using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServerTestClient
{
    public class CommandArgumentException : Exception
    {
        public CommandArgumentException(string message)
            : base (message)
        {

        }
    }
}
