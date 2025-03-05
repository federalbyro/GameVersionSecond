using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueFightGame
{
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
