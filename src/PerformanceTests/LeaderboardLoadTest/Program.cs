// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityModel.Client;
using System.Net.Http;
using System.Threading;
using System.IO;
using System.Linq;

namespace LeaderboardLoadTest
{
    public class Program
    {
        private readonly Dictionary<string, string> _userNameToPassword = new Dictionary<string, string>();
        private readonly TextWriter _log = Console.Out;

        public static void Main(string[] args)
        {
            //todo: add command line validation         

            int totalUsers = int.Parse(args[0]);
            int callsPerUser = int.Parse(args[1]);

            new Program().Run(totalUsers, callsPerUser);
        }

        private void Run(int totalUsers, int callsPerUser)
        {
            InititialiseUsers(totalUsers);

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            var gameSessions = new List<Tuple<Task, AutoPlayer>>();

            var startTime = DateTime.UtcNow;
            _log.WriteLine("load testing with {0} users, {1} calls per each...", totalUsers, callsPerUser);
            foreach (var userEntry in _userNameToPassword)
            {
                var player = new AutoPlayer(userEntry.Key, userEntry.Value, _log);

                //_log.WriteLine("starting player '{0}'...", userEntry.Key);                                     
                Task task = Task.Run(
                   () => player.PlayGameAsync(callsPerUser, cancellationToken));

                gameSessions.Add(Tuple.Create(task, player));
            }

            _log.WriteLine("waiting for load session to finish...");
            Task.WaitAll(gameSessions.Select(s => s.Item1).ToArray());
            //_log.WriteLine("all done.");                                                                       
            //Console.ReadLine();                                                                                

            _log.WriteLine("Statistics:");
            _log.WriteLine();
            foreach (var session in gameSessions)
            {
                _log.WriteLine("Player {0}", session.Item2.Id);
                foreach (string callName in session.Item2.CallNames)
                {
                    _log.WriteLine("  {0}", callName);
                    _log.WriteLine("    average call duration: {0}ms", session.Item2.GetAvgCallTime(callName));
                    _log.WriteLine("    calls/second: {0}", session.Item2.GetAvgCallsPerSecond(callName));
                }
                _log.WriteLine();
            }

            _log.WriteLine("total averages:");
            foreach (string callName in gameSessions.First().Item2.CallNames)
            {
                _log.WriteLine("  {0}", callName);
                _log.WriteLine("    average call duration: {0}ms",
                    gameSessions.Select(s => s.Item2.GetAvgCallTime(callName)).Average());
                _log.WriteLine("    calls/second: {0}",
                    gameSessions.Select(s => s.Item2.GetAvgCallsPerSecond(callName)).Average());
            }
            _log.WriteLine("finished in {0}", DateTime.UtcNow - startTime);

            //cancellationTokenSource.Cancel();                                                  
        }

        private void InititialiseUsers(int count)
        {
            for (int i = 0; i < count; i++)
            {
                string userName = "loadUser" + i;
                string password = userName;

                _userNameToPassword.Add(userName, password);
            }
        }
    }
}
