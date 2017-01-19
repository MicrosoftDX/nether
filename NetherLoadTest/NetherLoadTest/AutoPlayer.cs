using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NetherLoadTest
{
    class AutoPlayer
    {
        private static int s_counter;
        private readonly string _password;
        private readonly string _username;
        private readonly Random _random = new Random();
        private readonly string _playerInternalId;
        private readonly Stopwatch _sw = new Stopwatch();
        private readonly NetherClient _client;
        private readonly Dictionary<string, List<long>> _callTimes = new Dictionary<string, List<long>>();

        private static readonly Random s_rnd = new Random(DateTime.UtcNow.Millisecond);

        public AutoPlayer(string uri, string username, string password)
        {
            _playerInternalId = (s_counter++).ToString();
            _playerInternalId = _playerInternalId.PadLeft(5, '0');

            _username = username;
            _password = password;
            // TODO: those values should be injected via config, not hard coded
            _client = new NetherClient(uri, "resourceowner-test", "devsecret");
        }

        public List<string> CallNames => _callTimes.Keys.ToList();
        public double GetAvgCallTime(string callName)
        {
            return _callTimes[callName].Average();
        }

        public double GetAvgCallsPerSecond(string callName)
        {
            List<long> lst = _callTimes[callName];
            return (double)lst.Count / TimeSpan.FromMilliseconds(lst.Sum()).TotalSeconds;
        }

        public string Id => _playerInternalId;

        public async Task LoginUserNamePasswordAsync()
        {
            await _client.LoginUserNamePasswordAsync(_username, _password);
        }

        public async Task PostScoreAsync()
        {
            await _client.PostScoreAsync(_random.Next());
        }

        public async Task GetScoreAsync()
        {
            await _client.GetScoresAsync();
        }

        public async Task PlayGameAsync(int sessionsPerUser)
        {
            /*var response = await _client.LoginUserNamePasswordAsync(_username, _password);

            if (!response.IsSuccess)
            {                
                return;
            }*/

            // simulate leaderboard activity
            int callsMade = 0;
            while (callsMade++ < sessionsPerUser)
            {
                using (Measure("PostScore"))
                {
                    var callResult = await _client.PostScoreAsync(_random.Next());
                }

                await RandomDelay();

                using (Measure("GetScores"))
                {
                    var callResult = await _client.GetScoresAsync();
                }

                await RandomDelay();

                List<long> times;
                if (!_callTimes.TryGetValue("PostScore", out times))
                {
                    times = new List<long>();
                    _callTimes["PostScore"] = times;
                }
                else
                {
                    times.Add(_sw.ElapsedMilliseconds);
                }
            }
        }

        private IDisposable Measure(string callName)
        {
            return new InternalMeasure(callName, this);
        }

        private async Task RandomDelay()
        {
            await Task.Delay(s_rnd.Next(1000, 10000));
        }

        private class InternalMeasure : IDisposable
        {
            private string _callName;
            private AutoPlayer _master;

            public InternalMeasure(string callName, AutoPlayer master)
            {
                _callName = callName;
                _master = master;

                _master._sw.Restart();
            }

            public void Dispose()
            {
                _master._sw.Stop();

                List<long> times;
                if (!_master._callTimes.TryGetValue(_callName, out times))
                {
                    times = new List<long>();
                    _master._callTimes[_callName] = times;
                }
                times.Add(_master._sw.ElapsedMilliseconds);
            }
        }
    }
}
