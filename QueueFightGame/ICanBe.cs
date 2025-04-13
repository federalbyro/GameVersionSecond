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

    public interface ICanBeBuff
    {
        BuffType BuffType { get; }
        float DamageMultiplier { get; }
        bool ShouldBlockDamage(IUnit attacker);
        void RemoveBuff();
        IUnit ApplyBuffToUnit(IUnit unit);
    }

    public enum BuffType
    {
        None,
        Spear,
        Horse,
        Shield,
        Helmet
    }
}
