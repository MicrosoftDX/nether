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
        private readonly Random _random = new Random();
        private readonly AutoPlayer _player;
        private bool _loggedIn;

        public NetherUnitTest()
        {
            string username = "loadUser" + _random.Next(10000); // hard coded user names created for the load test in the memory store
            string password = username;
            _player = new AutoPlayer("http://localhost:5000", username, password);
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
