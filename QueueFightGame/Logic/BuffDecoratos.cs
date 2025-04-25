using System;

namespace QueueFightGame
{
    // Base Decorator (Optional but can be useful)
    public abstract class BuffDecoratorBase : ICanBeBuff
    {
        protected readonly StrongFighter _fighter;
        public abstract BuffType BuffType { get; }
        public virtual float DamageMultiplier => 1.0f;

        protected BuffDecoratorBase(StrongFighter fighter)
        {
            _fighter = fighter ?? throw new ArgumentNullException(nameof(fighter));
        }

        public virtual float GetModifiedProtection(IUnit attacker)
        {
            // By default, buff doesn't change protection unless overridden
            return _fighter.Protection;
        }

        public virtual void ApplyBuffEffect(StrongFighter fighter) { /* Visuals? */ }
        public virtual void RemoveBuffEffect(StrongFighter fighter) { /* Remove Visuals? */ }
    }

    // --- Specific Buffs ---

    public class SpearBuffDecorator : BuffDecoratorBase
    {
        public override BuffType BuffType => BuffType.Spear;
        public override float DamageMultiplier => 1.5f; // Increased damage

        public SpearBuffDecorator(StrongFighter fighter) : base(fighter) { }

        // Protection not affected by Spear
        public override float GetModifiedProtection(IUnit attacker) => _fighter.Protection;
    }

    public class HorseBuffDecorator : BuffDecoratorBase
    {
        public override BuffType BuffType => BuffType.Horse;
        public override float DamageMultiplier => 1.2f; // Slight damage increase

        public HorseBuffDecorator(StrongFighter fighter) : base(fighter) { }

        // Horse provides better defense (lower protection value means less damage taken)
        public override float GetModifiedProtection(IUnit attacker) => Math.Max(0.1f, _fighter.Protection - 0.2f); // Example: Improve protection by 20% points
    }

    public class ShieldBuffDecorator : BuffDecoratorBase
    {
        public override BuffType BuffType => BuffType.Shield;
        // No damage multiplier

        public ShieldBuffDecorator(StrongFighter fighter) : base(fighter) { }

        // Shield significantly improves defense
        public override float GetModifiedProtection(IUnit attacker) => Math.Max(0.1f, _fighter.Protection - 0.3f); // Example: Improve protection by 30% points
    }

    public class HelmetBuffDecorator : BuffDecoratorBase
    {
        public override BuffType BuffType => BuffType.Helmet;
        // No damage multiplier

        public HelmetBuffDecorator(StrongFighter fighter) : base(fighter) { }

        // Helmet improves defense specifically against Archers
        public override float GetModifiedProtection(IUnit attacker)
        {
            if (attacker is Archer)
            {
                return Math.Max(0.1f, _fighter.Protection - 0.4f); // Better protection vs Archers
            }
            return _fighter.Protection; // Normal protection otherwise
        }
    }
}