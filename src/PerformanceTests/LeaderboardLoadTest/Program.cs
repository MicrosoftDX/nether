// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityModel.Client;
using System.Net.Http;
using System.Threading;

namespace LeaderboardLoadTest
{
    public class Program
    {
        public static Dictionary<string, string> users = new Dictionary<string, string>
        {
            {"devuser", "devuser"},
            {"devadmin", "devadmin"},
            {"testuser", "testuser" },
            {"testuser1", "testuser1" },
            {"testuser2", "testuser2" },
            {"testuser3", "testuser3" }
        };

        public static void Main(string[] args)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            foreach (var userEntry in users)
            {
                var player = new AutoPlayer(userEntry.Key, userEntry.Value, Console.Out);

                var task = Task.Factory.StartNew(() => player.PlayGameAsync(cancellationToken));
            }
            Console.Read();
        }
    }
}
