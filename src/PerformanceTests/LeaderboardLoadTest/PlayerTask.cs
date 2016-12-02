using System.Threading.Tasks;

namespace LeaderboardLoadTest
{
    public class PlayerTask
    {
        public PlayerTask(AutoPlayer player, Task task)
        {
            Player = player;
            Task = task;
        }

        public AutoPlayer Player { get; }

        public Task Task { get; }
    }
}
