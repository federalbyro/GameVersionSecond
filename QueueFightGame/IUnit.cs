using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueFightGame
{
    internal interface IUnit
    {
        int Health { get; set; }
        float Protection { get; set; }
        float Damage { get; set; }
        void Attack();

    }
}
