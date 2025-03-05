using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueFightGame
{
    internal class Team
    {
        public Queue<IUnit> Fighters { get; private set; }
        public string TeamName { get; private set; }

        public Team(string teamName)
        {
            TeamName = teamName;
            Fighters = new Queue<IUnit>();
        }

        public void AddFighter(IUnit fighter)
        {
            throw new NotImplementedException("Метод AddFighter() еще не реализован.");
        }

        public IUnit GetNextFighter()
        {
            throw new NotImplementedException("Метод GetNextFighter() еще не реализован.");
        }

        public bool IsDefeated()
        {
            throw new NotImplementedException("Метод IsDefeated() еще не реализован.");
        }
    }
}
