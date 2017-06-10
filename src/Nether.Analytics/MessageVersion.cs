// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Nether.Analytics
{
    public struct MessageVersion
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Revision { get; set; }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Revision}";
        }

        public string Compatible
        {
            get
            {
                return $"{Major}.{Minor}.x";
            }
        }

        public static MessageVersion Parse(string value)
        {
            var parts = value.Split('.');
            if (parts.Length != 3)
                throw new ArgumentException("Message version should be three integers separated by dots (.): major.minor.revision");

            try
            {
                return new MessageVersion
                {
                    Major = int.Parse(parts[0]),
                    Minor = int.Parse(parts[1]),
                    Revision = int.Parse(parts[2])
                };
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Unable to parse version string as series of three integers", ex);
            }
        }
    }
}