using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueFightGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            WeakFighter weakFighter = new WeakFighter();
            StrongFighter strongFighter = new StrongFighter();
            Archer archer = new Archer();
            Healer healer = new Healer();

            weakFighter.Attack(strongFighter);
            archer.DoSpecialAction(strongFighter);
            healer.DoHeal(weakFighter);
        }
    }
}
