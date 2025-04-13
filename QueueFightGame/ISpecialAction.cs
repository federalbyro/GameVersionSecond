using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueFightGame
{
    public interface ISpecialActionHealer
    {
        int Range { get; }
        int Power { get; }

        void DoHeal(Team ownTeam);
    }

    public interface ISpecialActionArcher
    {
        int Range { get; }
        int Power { get; }

        void DoSpecialAttack(IUnit target, Team ownTeam);
    }

    public interface ISpecialActionMage
    {
        int CloneRange { get; }

        void DoClone(Team ownTeam);
    }
    public interface ISpecialActionWeakFighter
    {
        int BuffRange { get; }
        void DoBuff(Team ownTeam);
    }
}
