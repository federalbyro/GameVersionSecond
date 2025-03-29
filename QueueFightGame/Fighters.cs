using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueFightGame
{
    internal class WeakFighter : BaseUnit, ICanBeHealed
    {
        public WeakFighter() : base("WeakFighter", 100f, 0.7f, 40, 15) { }
    }

    internal class StrongFighter : BaseUnit
    {
        public StrongFighter() : base("StrongFighter", 100f, 0.5f, 60, 30) { }
    }

    internal class Healer : BaseUnit, ISpecialActionHealer
    {
        public int Range { get; private set; }
        public int Power { get; private set; }
        public Healer(string name) : base(name, 100f, 1f, 5, 10)
        {
            Range = 3;
            Power = 15;
        }

        public void DoHeal(Team ownTeam)
        {
            int healerIndex = ownTeam.QueueFighters.ToList().FindIndex(unit => unit == this);

            ICanBeHealed target = (ICanBeHealed)ownTeam.QueueFighters.ToList()
                .Where(unit => unit is ICanBeHealed && unit.Health < 100)
                .FirstOrDefault(unit => Math.Abs(ownTeam.QueueFighters.ToList().IndexOf(unit) - healerIndex) <= Range);

            Random random = new Random();
            int amountHealth = random.Next(0, Power + 1);

            if (target != null)
            {
                target.Health += amountHealth;
                if (target.Health > 100) target.Health = 100;

                Console.WriteLine($"{Name} лечит {((IUnit)target).Name}, восстанавливая {amountHealth} HP!");
            }
            else
            {
                Console.WriteLine($"{Name} не нашел раненых союзников в радиусе {Range}.");
            }
        }
    }

    internal class Archer : BaseUnit, ISpecialActionArcher, ICanBeHealed
    {
        public int Range { get; set; }
        public int Power { get; set ; }
        public Archer(string name) : base(name, 100f, 0.9f, 5, 25) 
        {
            Range = 3;
            Power = 15;
        }
        public void DoSpecialAttack(IUnit target, Team ownTeam)
        {
            int archerIndex = ownTeam.QueueFighters.ToList().FindIndex(unit => unit == this);

            if (archerIndex >= Range)
            {
                Console.WriteLine($"{Name} не может стрелять, его обзор закрыт!");
                return;
            }

            Random random = new Random();
            bool isHit = random.Next(100) < 70;

            if (isHit)
            {
                float newDamage = Power * target.Protection;
                Console.WriteLine($"{Name} стреляет в {target.Name} и попадает, нанося {newDamage} урона!");
                target.Health -= newDamage;
            }
            else
            {
                Console.WriteLine($"{Name} стреляет в {target.Name}, но промахивается!");
            }
        }
    }
    internal class StoneWall : BaseWall
    {
        public StoneWall() : base("Каменная стена", 200, 0.3f) { }
    }
}
