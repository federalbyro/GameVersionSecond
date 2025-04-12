using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueFightGame
{
    public abstract class BaseUnit : IUnit
    {
        public string Name { get; private set; }
        public int ID { get; private set; }
        public float Health { get; set; }
        public float Protection { get; private set; }
        public float Damage { get; private set; }
        public float Cost { get; private set; }
        public string Description { get; private set; }
        public Team Team { get; set; }

        public BaseUnit(string name, int id, float health, float protection, float damage, float cost, string description) 
        {
            Name = name;
            ID = id;
            Health = health;
            Protection = protection;
            Damage = damage;
            Cost = cost;
            Description = description;
            Team = null;
        }

        public virtual void Attack(IUnit target)
        {

            float newDamage = Damage * target.Protection;

            target.Health -= newDamage;

            Console.WriteLine($"Наносит урон {newDamage} {target.Name}");
        }
    }
    public class WallAdapter : BaseUnit
    {
        private BaseWall wall;

        public WallAdapter(BaseWall wall) : base(wall.Name, wall.Health, wall.Protection, 0, 30)
        {
            Team = null;
        }
        public override void Attack(IUnit target)
        {
            Console.WriteLine($"{Name} не может атаковать!");
        }
    }

    public abstract class BaseWall : IWall
    {
        public string Name { get; }
        public float Health { get; set; }
        public float Protection { get; set; }

        public BaseWall(string name, float health, float protection)
        {
            Name = name;
            Health = health;
            Protection = protection;
        }
    }
}
