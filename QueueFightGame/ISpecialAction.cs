using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueFightGame
{
    internal interface ISpecialActionHealer
    {
        int Range { get; }
        int Power { get; }

        void DoHeal(Team ownTeam);
    }

    internal interface ISpecialActionArcher
    {
        int Range { get; }
        int Power { get; }

        void DoSpecialAttack(IUnit target, Team ownTeam);
    }
}
