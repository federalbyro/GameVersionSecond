using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueFightGame
{
    internal abstract class BaseUnit : IUnit
    {
        public string Name { get; private set; }
        public float Health { get; set; }
        public float Protection { get; private set; }
        public float Damage { get; private set; }
        public Team Team { get; set; }

        public BaseUnit(string name, float health, float protection, float damage) 
        {
            Name = name;
            Health = health;
            Protection = protection;
            Damage = damage;
            Team = null;
        }

        public void Attack(IUnit target)
        {
            Console.WriteLine($"Attack to {target.Name}");

            float newDamage = Damage * target.Protection;

            target.Health -= newDamage;
        }
    }
}
