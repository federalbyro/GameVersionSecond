using System;
using System.Collections.Generic;
using System.Linq;

namespace QueueFightGame
{
    public class WeakFighter : BaseUnit, ICanBeHealed, ICanBeCloned, ISpecialActionWeakFighter
    {
        public int BuffRange { get; } = 1;
        private bool _hasAppliedBuff = false;
        private StrongFighter _knight;

        public WeakFighter(int ID) : base("WeakFighter", ID, 100f, 0.7f, 40, 15, "Оруженосец") { }

        public ICanBeCloned Clone()
        {
            return new WeakFighter(ID) { _hasAppliedBuff = this._hasAppliedBuff };
        }

        public void DoBuff(Team ownTeam)
        {
            if (_hasAppliedBuff || ownTeam == null || Health <= 0) return;

            // Находим ближайшего StrongFighter
            var knights = ownTeam.Fighters
                .Where(u => u is StrongFighter k && k.Health > 0)
                .Cast<StrongFighter>()
                .OrderBy(k => Math.Abs(ownTeam.Fighters.IndexOf(k) - ownTeam.Fighters.IndexOf(this)))
                .FirstOrDefault();

            if (knights != null && Math.Abs(ownTeam.Fighters.IndexOf(knights) - ownTeam.Fighters.IndexOf(this)) <= BuffRange)
            {
                _knight = knights;
                _knight.SetSquire(this);

                // Применяем случайный бафф
                var buffs = new[] { BuffType.Spear, BuffType.Horse, BuffType.Shield, BuffType.Helmet };
                var selectedBuff = buffs[new Random().Next(buffs.Length)];

                _knight.ApplyBuff(selectedBuff);
                _hasAppliedBuff = true;
            }
        }
    }
    public class StrongFighter : BaseUnit
    {
        private WeakFighter _squire;
        private ICanBeBuff _currentBuff;

        public StrongFighter(int ID) : base("StrongFighter", ID, 100f, 0.5f, 60, 30, "Сильный") { }

        public void SetSquire(WeakFighter squire)
        {
            _squire = squire;
            Console.WriteLine($"{Name} получает оруженосца {squire.Name}");
        }

        public void ApplyBuff(BuffType buffType)
        {
            if (_currentBuff != null && _currentBuff.BuffType == buffType)
                return; // Бафф такого типа уже активен

            switch (buffType)
            {
                case BuffType.Spear:
                    _currentBuff = new SpearBuffDecorator(this);
                    break;
                case BuffType.Horse:
                    _currentBuff = new HorseBuffDecorator(this);
                    break;
                case BuffType.Shield:
                    _currentBuff = new ShieldBuffDecorator(this);
                    break;
                case BuffType.Helmet:
                    _currentBuff = new HelmetBuffDecorator(this);
                    break;
                case BuffType.None:
                    _currentBuff = null;
                    break;
                default:
                    return; // Неизвестный тип баффа
            }

            if (_currentBuff != null)
            {
                Console.WriteLine($"{Name} получает бафф {buffType}");
            }
            else if (buffType == BuffType.None)
            {
                Console.WriteLine($"{Name} снимает все баффы");
            }
        }

        public void RemoveBuff()
        {
            if (_currentBuff != null)
            {
                Console.WriteLine($"{Name} теряет бафф {_currentBuff.BuffType}");
                _currentBuff = null;
            }
        }

        public override void Attack(IUnit target)
        {
            float damage = Damage * (_currentBuff?.DamageMultiplier ?? 1f) * target.Protection;
            target.Health -= damage;
            Console.WriteLine($"{Name} наносит {damage} урона {target.Name}");

            if (_currentBuff != null &&
               (_currentBuff.BuffType == BuffType.Spear || _currentBuff.BuffType == BuffType.Horse))
            {
                RemoveBuff();
            }
        }

        public bool HasBuff(BuffType buffType) =>
            _currentBuff != null && _currentBuff.BuffType == buffType;

        public bool ShouldBlockDamage(IUnit attacker) =>
            _currentBuff?.ShouldBlockDamage(attacker) ?? false;
    }

    public class Healer : BaseUnit, ISpecialActionHealer, ICanBeCloned
    {
        public int Range { get; private set; }
        public int Power { get; private set; }

        public Healer(string name, int ID) : base(name, ID, 100f, 1f, 5, 10, "Лекарь")
        {
            Range = 3;
            Power = 15;
        }

        public Healer(Healer prototype) : base(prototype.Name + "_clone", prototype.ID,
        prototype.Health, prototype.Protection, prototype.Damage, prototype.Cost, prototype.Description)
        {
            Range = prototype.Range;
            Power = prototype.Power;
        }

        public ICanBeCloned Clone()
        {
            return new Healer(this);
        }

        public void DoHeal(Team ownTeam)
        {
            if (ownTeam == null) throw new ArgumentNullException(nameof(ownTeam));

            var fighters = ownTeam.Fighters;
            int healerIndex = fighters.FindIndex(unit => unit == this);

            ICanBeHealed target = (ICanBeHealed)fighters
                .Where(unit => unit is ICanBeHealed && unit.Health < 100)
                .FirstOrDefault(unit => Math.Abs(fighters.IndexOf(unit) - healerIndex) <= Range);

            int amountHealth = new Random().Next(0, Power + 1);

            if (target != null)
            {
                target.Health = Math.Min(100, target.Health + amountHealth);
                Console.WriteLine($"{Name} лечит {((IUnit)target).Name}, восстанавливая {amountHealth} HP!");
            }
            else
            {
                Console.WriteLine($"{Name} не нашел раненых союзников в радиусе {Range}.");
            }
        }
    }

    public class Archer : BaseUnit, ISpecialActionArcher, ICanBeHealed, ICanBeCloned
    {
        public int Range { get; set; }
        public int Power { get; set; }

        public Archer(string name, int ID) : base(name, ID, 100f, 0.9f, 5, 25, "Лучник")
        {
            Range = 3;
            Power = 15;
        }

        public Archer(Archer prototype) : base(prototype.Name + "_clone", prototype.ID,
            prototype.Health, prototype.Protection, prototype.Damage, prototype.Cost, prototype.Description)
        {
            Range = prototype.Range;
            Power = prototype.Power;
        }

        public ICanBeCloned Clone()
        {
            return new Archer(this);
        }

        public void DoSpecialAttack(IUnit target, Team ownTeam)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (ownTeam == null) throw new ArgumentNullException(nameof(ownTeam));

            int archerIndex = ownTeam.Fighters.IndexOf(this);
            int targetIndex = ownTeam.Fighters.IndexOf(target);

            if (Math.Abs(archerIndex - targetIndex) > Range)
            {
                Console.WriteLine($"{Name} не может атаковать {target.Name} - цель вне досягаемости!");
                return;
            }

            bool isHit = new Random().Next(100) < 70;
            if (isHit)
            {
                float damage = Power * target.Protection;
                target.Health -= damage;
                Console.WriteLine($"{Name} стреляет в {target.Name} и попадает, нанося {damage} урона!");
            }
            else
            {
                Console.WriteLine($"{Name} стреляет в {target.Name}, но промахивается!");
            }
        }
    }

    public class Mage : BaseUnit, ICanBeHealed, ISpecialActionMage
    {
        public int CloneRange { get; } = 2;

        public Mage(string name, int ID) : base(name, ID, 100f, 0.8f, 20, 25, "Маг") { }

        public void DoClone(Team ownTeam)
        {
            if (ownTeam == null) throw new ArgumentNullException(nameof(ownTeam));

            if (new Random().Next(100) >= 5)                
            {
                return;
            }

            var fighters = ownTeam.Fighters;
            int mageIndex = fighters.IndexOf(this);

            var possibleTargets = fighters
                .Where((u, index) =>
                    index != mageIndex &&
                    Math.Abs(index - mageIndex) <= CloneRange &&
                    u is ICanBeCloned)
                .ToList();

            if (!possibleTargets.Any())
            {
                Console.WriteLine($"{Name} не нашел подходящих целей для клонирования рядом!");
                return;
            }

            var target = possibleTargets[new Random().Next(possibleTargets.Count)];
            var clone = ((ICanBeCloned)target).Clone();

            if (clone is IUnit unitClone)
            {
                ownTeam.Fighters.Insert(mageIndex + 1, unitClone);
                Console.WriteLine($"{Name} успешно создал клона {unitClone.Name}!");
            }
        }
    }

    public class StoneWall : BaseWall
    {
        public StoneWall() : base("Каменная стена", 200, 0.3f) { }
    }
}