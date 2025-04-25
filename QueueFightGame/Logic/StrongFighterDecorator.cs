// BuffDecorators.cs
/*
 * using System;

namespace QueueFightGame
{
    public class SpearBuffDecorator : ICanBeBuff
    {
        private readonly StrongFighter _fighter;

        public BuffType BuffType => BuffType.Spear;
        public float DamageMultiplier => 1.5f;

        public SpearBuffDecorator(StrongFighter fighter)
        {
            _fighter = fighter ?? throw new ArgumentNullException(nameof(fighter));
        }

        public bool ShouldBlockDamage(IUnit attacker) => false;

        public void RemoveBuff()
        {
            _fighter.RemoveBuff();
        }
    }    

    public class HorseBuffDecorator : ICanBeBuff
    {
        private readonly StrongFighter _fighter;

        public BuffType BuffType => BuffType.Horse;
        public float DamageMultiplier => 1.3f;

        public HorseBuffDecorator(StrongFighter fighter)
        {
            _fighter = fighter ?? throw new ArgumentNullException(nameof(fighter));
        }

        public bool ShouldBlockDamage(IUnit attacker) => true;

        public void RemoveBuff()
        {
            _fighter.RemoveBuff();
        }
    }

    public class ShieldBuffDecorator : ICanBeBuff
    {
        private readonly StrongFighter _fighter;

        public BuffType BuffType => BuffType.Shield;
        public float DamageMultiplier => 1f;

        // Реализация IUnit
        public string Name => _fighter.Name;
        public int ID => _fighter.ID;
        public float Health { get => _fighter.Health; set => _fighter.Health = value; }
        public float Protection => _fighter.Protection;
        public float Damage => _fighter.Damage * DamageMultiplier;
        public float Cost => _fighter.Cost;
        public string Description => _fighter.Description;
        public Team Team { get => _fighter.Team; set => _fighter.Team = value; }

        public ShieldBuffDecorator(StrongFighter fighter)
        {
            _fighter = fighter;
        }

        public bool ShouldBlockDamage(IUnit attacker) => true;

        public void RemoveBuff()
        {
            _fighter.RemoveBuff();
        }

        public void Attack(IUnit target)
        {
            float damage = Damage * target.Protection;
            target.Health -= damage;
            Console.WriteLine($"{Name} наносит {damage} урона {target.Name}");
        }
    }

    public class HelmetBuffDecorator : ICanBeBuff
    {
        private readonly StrongFighter _fighter;

        public BuffType BuffType => BuffType.Helmet;
        public float DamageMultiplier => 1f;

        public HelmetBuffDecorator(StrongFighter fighter)
        {
            _fighter = fighter ?? throw new ArgumentNullException(nameof(fighter));
        }

        public bool ShouldBlockDamage(IUnit attacker) => attacker is Archer;

        public void RemoveBuff()
        {
            _fighter.RemoveBuff();
        }
    }
}

*/