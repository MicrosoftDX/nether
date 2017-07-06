// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.IO;

namespace Nether.Test.ConsoleClient
{
    public class GamerTagProvider : IGamerTagProvider
    {
        private Random _rnd = new Random();

        private string[] _gamerTags1;
        private string[] _gamerTags2;
        private string[] _gamerTags3;

        private ConcurrentDictionary<string, bool> _gamerTagsInUse = new ConcurrentDictionary<string, bool>();

        public GamerTagProvider(string gamerTagsFile1, string gamerTagsFile2, string gamerTagsFile3)
        {
            _gamerTags1 = ReadFile(gamerTagsFile1);
            _gamerTags2 = ReadFile(gamerTagsFile2);
            _gamerTags3 = ReadFile(gamerTagsFile3);
        }

        private string[] ReadFile(string gamerTagsFile)
        {
            var fileContent = File.ReadAllText(gamerTagsFile);

            return fileContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }

        public string GetGamerTag()
        {
            bool useThird = false;

            while (true)
            {
                string gamerTag;

                if (!useThird)
                    gamerTag = _gamerTags1.TakeRandom() + _gamerTags2.TakeRandom();
                else
                    gamerTag = _gamerTags1.TakeRandom() + _gamerTags2.TakeRandom() + _gamerTags3.TakeRandom();

                lock (_gamerTagsInUse)
                {
                    if (_gamerTagsInUse.TryGetValue(gamerTag, out bool val))
                    {
                        useThird = true;
                        continue;
                    }

                    _gamerTagsInUse.TryAdd(gamerTag, true);
                }

                return gamerTag;
            }
        }

        public void ReturnGamerTag(string gamerTag)
        {
            _gamerTagsInUse.TryRemove(gamerTag, out bool val);
        }
    }
}