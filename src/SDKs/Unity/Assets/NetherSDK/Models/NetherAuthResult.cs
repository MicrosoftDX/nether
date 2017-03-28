using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetherSDK.Models
{
    [Serializable]
    public class NetherAuthResult
    {
        public string access_token;
        public int expires_in;
        public string token_type;
    }
}
