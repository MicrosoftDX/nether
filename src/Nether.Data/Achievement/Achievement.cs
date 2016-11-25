// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Data.Achievement
{
    //to do:
    // This is planned for M2
    // just early thinking

    public enum State { Hidden, Revealed, Unlocked }

    public class Achievement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }
        public int Order { get; set; }
        public bool Incremental { get; set; }
        public State DefaultState { get; set; }
    }
}

