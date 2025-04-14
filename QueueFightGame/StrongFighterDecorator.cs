// BuffDecorators.cs
namespace QueueFightGame
{
    public class SpearBuffDecorator : ICanBeBuff
    {
        private readonly StrongFighter _fighter;

        public BuffType BuffType => BuffType.Spear;
        public float DamageMultiplier => 1.5f;

        public SpearBuffDecorator(StrongFighter fighter)
        {
            _fighter = fighter;
        }

        public bool ShouldBlockDamage(IUnit attacker) => false;

        public void RemoveBuff()
        {
            _fighter.RemoveBuff();
        }

        public IUnit ApplyBuffToUnit(IUnit unit) => this;
    }

    public class HorseBuffDecorator : ICanBeBuff
    {
        private readonly StrongFighter _fighter;

        public BuffType BuffType => BuffType.Horse;
        public float DamageMultiplier => 1.3f;

        public HorseBuffDecorator(StrongFighter fighter)
        {
            _fighter = fighter;
        }

        public bool ShouldBlockDamage(IUnit attacker) => true;

        public void RemoveBuff()
        {
            _fighter.RemoveBuff();
        }

        public IUnit ApplyBuffToUnit(IUnit unit) => this;
    }

    public class ShieldBuffDecorator : ICanBeBuff
    {
        private readonly StrongFighter _fighter;

        public BuffType BuffType => BuffType.Shield;
        public float DamageMultiplier => 1f;

        public ShieldBuffDecorator(StrongFighter fighter)
        {
            _fighter = fighter;
        }

        public bool ShouldBlockDamage(IUnit attacker) => true;

        public void RemoveBuff()
        {
            _fighter.RemoveBuff();
        }

        public IUnit ApplyBuffToUnit(IUnit unit) => this;
    }

    public class HelmetBuffDecorator : ICanBeBuff
    {
        private readonly StrongFighter _fighter;

        public BuffType BuffType => BuffType.Helmet;
        public float DamageMultiplier => 1f;

        public HelmetBuffDecorator(StrongFighter fighter)
        {
            _fighter = fighter;
        }

        public bool ShouldBlockDamage(IUnit attacker) => attacker is Archer;

        public void RemoveBuff()
        {
            _fighter.RemoveBuff();
        }

        public IUnit ApplyBuffToUnit(IUnit unit) => this;
    }
}