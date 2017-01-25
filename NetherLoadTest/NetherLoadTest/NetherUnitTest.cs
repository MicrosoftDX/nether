// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Net.Http;
using System.Net;


namespace NetherLoadTest
{
    [TestClass]
    public class NetherUnitTest
    {
        private Random _random = new Random();
        private AutoPlayer _player;
        private bool _loggedIn;



        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Init()
        {
            string username = "loadUser" + _random.Next(10000); // hard coded user names created for the load test in the memory store
            string password = username;
            var baseUrl = (string)TestContext.Properties["BaseUrl"] ?? "http://localhost:5000";
            _player = new AutoPlayer(baseUrl, username, password);
            _loggedIn = false;
        }

        private async Task LoginUserNamePasswordAsync()
        {
            if (!_loggedIn)
            {
                await _player.LoginUserNamePasswordAsync();
                _loggedIn = true;
            }
        }

        [TestMethod]
        public async Task PlayGame()
        {
            await LoginUserNamePasswordAsync();
            int games = _random.Next(100);
            await _player.PlayGameAsync(games);
        }

        [TestMethod]
        public async Task PostScore()
        {
            await LoginUserNamePasswordAsync();
            await _player.PostScoreAsync();
        }

        [TestMethod]
        public async Task GetScore()
        {
            await LoginUserNamePasswordAsync();
            await _player.GetScoreAsync();
        }
    }
}
