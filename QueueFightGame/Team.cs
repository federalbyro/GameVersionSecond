using System;
using System.Collections.Generic;
using System.Linq;

namespace QueueFightGame
{
    public class Team
    {
        public List<IUnit> Fighters { get; private set; }
        public string TeamName { get; private set; }
        public float Money { get; private set; }

        public Team(string teamName, float money)
        {
            TeamName = teamName;
            Fighters = new List<IUnit>();
            Money = money;
        }

        public void AddFighter(IUnit fighter)
        {
            if (Money >= fighter.Cost)
            {
                fighter.Team = this;
                Money -= fighter.Cost;
                Fighters.Add(fighter);
                Console.WriteLine($"Add {fighter.Name} to {TeamName}");
            }
            else
            {
                Console.WriteLine($"{TeamName} don't have enough money to add {fighter.Name}");
            }
        }

        public bool HasFighters()
        {
            return Fighters.Count > 0;
        }

        public void ShowTeam()
        {
            Console.WriteLine($"\n--- Команда: {TeamName} ---");
            Console.WriteLine($"Денег осталось: {Money}");
            Console.WriteLine("Бойцы в команде:");

            if (Fighters.Count == 0)
            {
                Console.WriteLine("  Нет бойцов в команде");
                return;
            }

            for (int i = 0; i < Fighters.Count; i++)
            {
                var fighter = Fighters[i];
                Console.WriteLine($"  {i + 1}. {fighter.Name} | HP: {fighter.Health} | Урон: {fighter.Damage} | Защита: {fighter.Protection}");
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
            return Fighters.First();
        }

        public void RemoveFighter()
        {
            if (HasFighters())
            {
                IUnit removedFighter = Fighters[0];
                Fighters.RemoveAt(0);
                Console.WriteLine($"{removedFighter.Name} покинул команду {TeamName}.");
            }
        }

        // Дополнительные полезные методы для работы с List
        public void InsertFighter(int position, IUnit fighter)
        {
            if (position < 0 || position > Fighters.Count)
            {
                Console.WriteLine($"Невозможно добавить бойца на позицию {position}");
                return;
            }

            fighter.Team = this;
            Fighters.Insert(position, fighter);
            Console.WriteLine($"Add {fighter.Name} to {TeamName} at position {position + 1}");
        }

        public IUnit GetFighterAt(int position)
        {
            if (position < 0 || position >= Fighters.Count)
                return null;

            return Fighters[position];
        }
    }
}