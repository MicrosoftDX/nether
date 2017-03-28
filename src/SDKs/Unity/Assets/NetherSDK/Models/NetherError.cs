using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetherSDK.Models
{
    [Serializable]
    public class NetherErrorResult
    {
        public Error error;
    }

    [Serializable]
    public class Error
    {
        public string code;
        public string message;
        public ErrorDetails[] details;
    }

    [Serializable]
    public class ErrorDetails
    {
        public string target;
        public string message;
    }
}
