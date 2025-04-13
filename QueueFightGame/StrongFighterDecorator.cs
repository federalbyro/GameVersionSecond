using System;

namespace QueueFightGame
{
    public abstract class StrongFighterDecorator : IUnit, ICanBeBuff
    {
        protected readonly IUnit _decoratedUnit;

        public BuffType BuffType { get; protected set; }
        public float DamageMultiplier { get; protected set; } = 1f;

        // Делегируем все свойства оригинальному юниту
        public string Name => _decoratedUnit.Name;
        public int ID => _decoratedUnit.ID;
        public float Health
        {
            get => _decoratedUnit.Health;
            set => _decoratedUnit.Health = value;
        }
        public float Protection => _decoratedUnit.Protection;
        public float Damage => _decoratedUnit.Damage * DamageMultiplier;
        public float Cost => _decoratedUnit.Cost;
        public string Description => _decoratedUnit.Description;
        public Team Team
        {
            get => _decoratedUnit.Team;
            set => _decoratedUnit.Team = value;
        }

        protected StrongFighterDecorator(IUnit unit)
        {
            _decoratedUnit = unit ?? throw new ArgumentNullException(nameof(unit));
        }

        public virtual void Attack(IUnit target)
        {
            if (target == null)
            {
                Console.WriteLine("Цель для атаки не указана!");
                return;
            }

            float damage = CalculateDamage(target);
            target.Health -= damage;

            Console.WriteLine($"{Name} наносит {damage} урона {target.Name}");

            RemoveBuffAfterAttack();
        }

        protected virtual float CalculateDamage(IUnit target)
        {
            return Damage * target.Protection;
        }

        protected virtual void RemoveBuffAfterAttack()
        {
            if (BuffType == BuffType.Spear || BuffType == BuffType.Horse)
            {
                RemoveBuff();
            }
        }

        public abstract bool ShouldBlockDamage(IUnit attacker);

        public virtual void RemoveBuff()
        {
            if (_decoratedUnit is StrongFighter fighter)
            {
                fighter.RemoveBuff();
            }
        }

        public IUnit ApplyBuffToUnit(IUnit unit)
        {
            return this;
        }
    }
}