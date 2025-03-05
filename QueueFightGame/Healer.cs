using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueFightGame
{
    internal class Healer : BaseUnit, ISpecialActionHealer
    {
        public int Range { get; private set; }
        public int Power { get; private set; }
        public Healer() : base ("Healer", 30f, 0.9f, 5)
        {
            Range = 3;
            Power = 5;
        }

        public void DoHeal(ICanBeHealed target)
        {
            Console.WriteLine($"Healer doheal {target.Name}");
        }
    }
}
