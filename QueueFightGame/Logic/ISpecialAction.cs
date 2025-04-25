using System.Collections.Generic;

namespace QueueFightGame
{
    // Combine interfaces for clarity or keep separate if preferred
    public interface ISpecialActionUnit : IUnit
    {
        bool HasUsedSpecial { get; set; } // To limit certain actions per turn/battle
        int SpecialActionChance { get; } // Probability (0-100)
        void PerformSpecialAction(Team ownTeam, Team enemyTeam, ILogger logger, CommandManager commandManager); // Unified method
    }

    // Specific interfaces can still exist for type checking if needed,
    // but the core logic can be driven by ISpecialActionUnit.
    public interface ISpecialActionHealer : ISpecialActionUnit
    {
        int HealRange { get; }
        int HealPower { get; }
    }

    public interface ISpecialActionArcher : ISpecialActionUnit
    {
        int AttackRange { get; }
        int AttackPower { get; }
    }

    public interface ISpecialActionMage : ISpecialActionUnit
    {
        int CloneRange { get; }
    }

    public interface ISpecialActionWeakFighter : ISpecialActionUnit
    {
        int BuffRange { get; }
        bool HasAppliedBuff { get; } // Specific state for squire
    }
}