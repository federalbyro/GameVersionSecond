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
        public float Money { get; private set; }

        public Team(string teamName, float money)
        {
            TeamName = teamName;
            QueueFighters = new Queue<IUnit>();
            Money = money;
        }

        public void AddFighter(IUnit fighter)
        {
            if (this.Money >= fighter.Cost)
            {
                fighter.Team = this;
                this.Money -= fighter.Cost;
                QueueFighters.Enqueue(fighter);
                Console.WriteLine($"Add {fighter.Name} to {this.TeamName}");
            }
            else
            {
                Console.WriteLine($"{this.TeamName} don't have enough money to add {fighter.Name}");
            }
        }

        public bool HasFighters()
        {
            return QueueFighters.Count > 0;
        }

        public void ShowTeam()
        {
            Console.WriteLine($"\n--- Команда: {TeamName} ---");
            Console.WriteLine($"Денег осталось: {Money}");
            Console.WriteLine("Бойцы в очереди:");

            if (QueueFighters.Count == 0)
            {
                Console.WriteLine("  Нет бойцов в команде");
                return;
            }

            int position = 1;
            foreach (var fighter in QueueFighters)
            {
                Console.WriteLine($"  {position}. {fighter.Name} | HP: {fighter.Health} | Урон: {fighter.Damage} | Защита: {fighter.Protection}");
                position++;
            }

            Console.WriteLine();
        }

        public IUnit GetNextFighter()
        {
            if (!HasFighters())
            {
                Console.WriteLine($"Команда {TeamName} больше не имеет бойцов!");
                return null;
            }
            return QueueFighters.Peek();
        }

        public void RemoveFighter()
        {
            if (HasFighters())
            {
                IUnit removedFighter = QueueFighters.Dequeue();
                Console.WriteLine($"{removedFighter.Name} покинул команду {TeamName}.");
            }
        }

    }
}
