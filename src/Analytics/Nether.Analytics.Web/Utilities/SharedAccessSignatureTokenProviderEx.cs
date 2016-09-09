using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Nether.Analytics.Web.Utilities
{
    public class SharedAccessSignatureTokenProviderEx
    {
        // TODO: Replace with SDK version, when supported from within .NET Core
        public static string GetSharedAccessSignature(string keyName, string sharedAccessKey, string resource, TimeSpan tokenTimeToLive)
        {
            var fromEpochStart = DateTime.UtcNow - new DateTime(1970, 1, 1);
            var expiry = Convert.ToString((int)fromEpochStart.TotalSeconds + tokenTimeToLive.TotalSeconds);
            var stringToSign = WebUtility.UrlEncode(resource) + "\n" + expiry;
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(sharedAccessKey));

            var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
        
            var token = String.Format(CultureInfo.InvariantCulture, 
                "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}",
                WebUtility.UrlEncode(resource), 
                WebUtility.UrlEncode(signature), 
                expiry, 
                keyName);

            return token;
        }
    }
}
