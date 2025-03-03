using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueFightGame
{
    internal abstract class BaseUnit : IUnit
    {
        public string Name { get => throw new NotImplementedException(); private set => throw new NotImplementedException(); };
        public int Health { get => throw new NotImplementedException(); private set => throw new NotImplementedException(); }
        public float Protection { get => throw new NotImplementedException(); private set => throw new NotImplementedException(); }
        public float Damage { get => throw new NotImplementedException(); private set => throw new NotImplementedException(); }

        public BaseUnit(string name, int health, float protection, float damage) 
        {
            Name = name;
            Health = health;
            Protection = protection;
            Damage = damage;
        }

        public void Attack(IUnit target)
        {
            throw new NotImplementedException();
        }
    }
}
