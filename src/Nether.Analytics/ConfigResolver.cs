//using System;
//using System.Configuration;
//using System.Collections.Generic;
//using System.Text;

//namespace Nether.Analytics
//{
//    //TODO: Move into library that can be used by all Nether libraries
//    public static class ConfigResolver
//    {
//        public static string Resolve(string name)
//        {
//            var configVar = Environment.GetEnvironmentVariable(name);
//            return configVar ?? ConfigurationManager.AppSettings[name].ToString();
//        }
//    }
//}
