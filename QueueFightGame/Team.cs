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
            fighter.Team = this;
            QueueFighters.Enqueue(fighter);
            Console.WriteLine($"Add {fighter.Name} to {this.TeamName}");
        }

        public IUnit ReplaceFighter()
        {
            throw new NotImplementedException();
        }

        public bool IsDefeated()
        {
            return QueueFighters.Count == 0;
        }
    }
}
