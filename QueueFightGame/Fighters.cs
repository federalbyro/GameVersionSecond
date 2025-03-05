using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueFightGame
{
    internal class WeakFighter : BaseUnit, ICanBeHealed
    {
        public WeakFighter() : base("WeakFighter", 100f, 0.8f, 25) { }
    }

    internal class StrongFighter : BaseUnit
    {
        public StrongFighter() : base("StrongFighter", 150f, 0.5f, 50) { }
    }

    internal class Healer : BaseUnit, ISpecialActionHealer
    {
        public int Range { get; private set; }
        public int Power { get; private set; }
        public Healer() : base("Healer", 30f, 0.9f, 5)
        {
            Range = 3;
            Power = 5;
        }

        public void DoHeal(ICanBeHealed target)
        {
            Console.WriteLine($"Healer doheal {target.Name}");
        }
    }

    internal class Archer : BaseUnit, ISpecialAction, ICanBeHealed
    {
        public int Range { get; set; }
        public int Power { get; set ; }
        public Archer() : base("Archer", 20f, 0.9f, 15) 
        {
            Range = 3;
            Power = 3;
        }
        public void DoSpecialAction(IUnit target)
        {
            Console.WriteLine($"Archer attaks {target.Name}");
        }
    }
}
