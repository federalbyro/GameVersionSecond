using System;
using System.Collections.Generic;
using System.Linq;

namespace QueueFightGame
{
    public class WeakFighter : BaseUnit, ICanBeHealed, ICanBeCloned
    {
        public WeakFighter(int ID) : base("WeakFighter", ID, 100f, 0.7f, 40, 15, "Слабый боец") { }
        public WeakFighter(WeakFighter prototype) : base(prototype.Name + "_clone", prototype.ID,
            prototype.Health, prototype.Protection, prototype.Damage, prototype.Cost, prototype.Description)
        { }

        public ICanBeCloned Clone()
        {
            return new WeakFighter(this);
        }
    }

    public class StrongFighter : BaseUnit
    {
        public StrongFighter(int ID) : base("StrongFighter", ID, 100f, 0.5f, 60, 30, "Сильный") { }
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

            if (new Random().Next(100) >= 20)                
            {
                Console.WriteLine($"{Name} попытался создать клона, но заклинание не сработало!");
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