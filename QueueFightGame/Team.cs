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

        public bool HasFighters()
        {
            return QueueFighters.Count > 0;
        }

        public IUnit GetCurrentFighter()
        {
            if (QueueFighters.Count == 0)
            {
                Console.WriteLine($"Команда {TeamName} больше не имеет бойцов!");
                return null;
            }
            return QueueFighters.Peek();
        }

        public void RemoveFighter()
        {
            if (QueueFighters.Count > 0)
            {
                IUnit removedFighter = QueueFighters.Dequeue();
                Console.WriteLine($"{removedFighter.Name} покинул команду {TeamName}.");
            }
        }

    }
}
