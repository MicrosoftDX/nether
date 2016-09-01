using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEventsGenerator
{
    class Player
    {
        public string Name { get; set; }

        public Location PlayerLocation { get; set; }
        public Player(string name, Location playerLocation)
        {
            Name = name;
            PlayerLocation = playerLocation;
        }
    }
}
