using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueFightGame
{
    public interface IUnit
    {
        string Name { get; }
        int ID { get;  }
        float Health { get; set; }
        float Protection { get; }
        float Damage { get; }
        float Cost { get; }
        string Description { get; }
        Team Team { get; set; }

        void Attack(IUnit target);
    }

    public interface IWall
    {
        string Name { get; }
        float Health { get; set; }
        float Protection { get; }
    }
}
