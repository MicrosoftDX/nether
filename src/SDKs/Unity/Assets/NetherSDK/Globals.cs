using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetherSDK
{
    public class Globals
    {
        public static bool DebugFlag { get; set; }
        public static readonly string Authorization = "Authorization";
        public static readonly string Accept = "Accept";
        public static readonly string Content_Type = "Content-Type";
        public static readonly string ApplicationJson = "application/json";
        public static readonly string ErrorOccurred = "Error occurred";
        public static readonly string ApplicationFormUrlEncoded = "application/x-www-form-urlencoded";
        public static readonly string LibraryVersion = "0.0.1";
    }
}