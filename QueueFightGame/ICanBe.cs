using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueFightGame
{
    public interface ICanBeHealed
    {
        string Name { get; }
        float Health { get; set; }
    }
    public interface ICanBeCloned : IUnit
    {
        ICanBeCloned Clone();
    }
}
