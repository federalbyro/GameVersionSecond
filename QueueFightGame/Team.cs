using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueFightGame
{
    internal class Team
    {
        public Queue<IUnit> QueueFighters { get; private set; }
        public string TeamName { get; private set; }

        public Team(string teamName)
        {
            TeamName = teamName;
            QueueFighters = new Queue<IUnit>();
        }

        public void AddFighter(IUnit fighter)
        {

        }

        public IUnit ReplaceFighter()
        {

        }

        public bool IsDefeated()
        {
            return QueueFighters.Count == 0;
        }
    }
}
