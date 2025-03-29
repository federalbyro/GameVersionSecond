using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueFightGame
{
    internal interface ICanBeHealed
    {
        string Name { get; }
        float Health { get; set; }
    }
    internal interface ICanBeCloned
    {
        //string Name { get; }
        //float Health { get; set; }
    }
}
