using System;
using System.Linq;
using System.Collections.Generic;

namespace QueueFightGame
{
    // --- WeakFighter ---
    public class WeakFighter : BaseUnit, ICanBeHealed, ICanBeCloned, ISpecialActionWeakFighter
    {
        private bool _hasAppliedBuff = false;
        private StrongFighter _knightToBuff = null; // Remember knight if found

        public int BuffRange { get; private set; }
        public bool HasAppliedBuff => _hasAppliedBuff;
        public override int SpecialActionChance => 100; // Always tries if conditions met

        public WeakFighter() : base(nameof(WeakFighter))
        {
            BuffRange = UnitConfig.Stats[nameof(WeakFighter)].BuffRange ?? 1;
        }

        // Cloning constructor
        private WeakFighter(WeakFighter original) : base(original)
        {
            this.BuffRange = original.BuffRange;
            this._hasAppliedBuff = original._hasAppliedBuff; // Clone keeps buff status? Or reset? Reset seems fairer.
            this._hasAppliedBuff = false;
        }

        public ICanBeCloned Clone()
        {
            return new WeakFighter(this);
        }

        // Special Action Logic
        public override void PerformSpecialAction(Team ownTeam, Team enemyTeam, ILogger logger, CommandManager commandManager)
        {
            TryApplyBuff(ownTeam, logger, commandManager);
            HasUsedSpecial = true; // Mark as used for this turn cycle if needed
        }

        private void TryApplyBuff(Team ownTeam, ILogger logger, CommandManager commandManager)
        {
            if (_hasAppliedBuff || this.Health <= 0) return;

            int myIndex = ownTeam.Fighters.IndexOf(this);
            if (myIndex < 0) return; // Should not happen

            // Find nearest StrongFighter within range that doesn't have a buff needing removal (Spear/Horse)
            _knightToBuff = ownTeam.Fighters
                .OfType<StrongFighter>()
                .Where(k => k.Health > 0 && Math.Abs(ownTeam.Fighters.IndexOf(k) - myIndex) <= BuffRange)
                .OrderBy(k => Math.Abs(ownTeam.Fighters.IndexOf(k) - myIndex))
                .FirstOrDefault();

            if (_knightToBuff != null)
            {
                // Create and execute buff command
                var buffCommand = new SquireBuffCommand(this, _knightToBuff, ownTeam, logger);
                commandManager.ExecuteCommand(buffCommand);
                // _hasAppliedBuff will be set by the command's Execute method
            }
        }

        // Method called by SquireBuffCommand to finalize buff application
        public void MarkBuffApplied(StrongFighter knight)
        {
            _knightToBuff = knight; // Ensure reference is set
            _hasAppliedBuff = true;
        }

        // Method called by SquireBuffCommand's Undo
        public void UnmarkBuffApplied()
        {
            _hasAppliedBuff = false;
            // Optionally clear _knightToBuff = null; if needed
        }
    }

    // --- StrongFighter ---
    public class StrongFighter : BaseUnit, ICanBeHealed // Not cloneable by default
    {
        private ICanBeBuff _currentBuff = null;
        private WeakFighter _squire = null; // Reference to the squire who buffed this knight

        public StrongFighter() : base(nameof(StrongFighter)) { }

        public void SetSquire(WeakFighter squire)
        {
            _squire = squire;
            // Log handled by command
        }

        public WeakFighter GetSquire() => _squire;

        public void ApplyBuff(ICanBeBuff buff, ILogger logger)
        {
            if (_currentBuff != null && _currentBuff.BuffType != BuffType.None)
            {
                // Maybe allow replacing buffs? For now, log and do nothing or remove old one first.
                logger.Log($"{Name} уже имеет бафф {_currentBuff.BuffType}. Новый бафф {buff.BuffType} не применен.");
                return;
            }
            _currentBuff = buff;
            _currentBuff?.ApplyBuffEffect(this); // Apply visual effects if any
            logger.Log($"{Name} получает бафф {buff.BuffType} от {_squire?.Name ?? "кого-то"}.");
        }

        public void RemoveBuff(ILogger logger)
        {
            if (_currentBuff != null && _currentBuff.BuffType != BuffType.None)
            {
                logger.Log($"{Name} теряет бафф {_currentBuff.BuffType}.");
                _currentBuff?.RemoveBuffEffect(this); // Remove visual effects
                _currentBuff = null;
                _squire = null; // Squire link is broken when buff is removed
            }
        }

        public BuffType CurrentBuffType => _currentBuff?.BuffType ?? BuffType.None;

        public override void Attack(IUnit target, ILogger logger)
        {
            float damageMultiplier = _currentBuff?.DamageMultiplier ?? 1.0f;
            float targetProtection = target.Protection;

            // Check if target has protection buff against this attacker type (relevant if target is also a StrongFighter)
            // This check is a bit complex here, might need refinement based on exact buff interactions desired.
            // Let's simplify: apply attacker's damage multiplier first.

            float baseDamage = this.Damage * damageMultiplier;
            float damageDealt = Math.Max(1, baseDamage * (1.0f - targetProtection));

            target.Health -= damageDealt;
            logger.Log($"{this.Name} ({this.Team.TeamName}){(CurrentBuffType != BuffType.None ? $" [{CurrentBuffType}]" : "")} атакует {target.Name} ({target.Team.TeamName}) и наносит {damageDealt:F1} урона. Осталось здоровья у {target.Name}: {target.Health:F1}");

            // Remove one-time buffs after attack
            if (_currentBuff != null && (CurrentBuffType == BuffType.Spear || CurrentBuffType == BuffType.Horse))
            {
                RemoveBuff(logger);
            }
        }

        // Override PerformSpecialAction if StrongFighter has its own special ability later
    }


    // --- Healer ---
    public class Healer : BaseUnit, ICanBeHealed, ICanBeCloned, ISpecialActionHealer
    {
        public int HealRange { get; private set; }
        public int HealPower { get; private set; }
        public override int SpecialActionChance => 40; // 40% chance to attempt heal

        public Healer() : base(nameof(Healer))
        {
            HealRange = UnitConfig.Stats[nameof(Healer)].Range ?? 1;
            HealPower = UnitConfig.Stats[nameof(Healer)].Power ?? 15;
        }

        // Cloning constructor
        private Healer(Healer original) : base(original)
        {
            this.HealRange = original.HealRange;
            this.HealPower = original.HealPower;
        }

        public ICanBeCloned Clone()
        {
            return new Healer(this);
        }

        // Special Action Logic
        public override void PerformSpecialAction(Team ownTeam, Team enemyTeam, ILogger logger, CommandManager commandManager)
        {
            if (HasUsedSpecial || Health <= 0) return;

            if (new Random().Next(100) < SpecialActionChance)
            {
                TryHeal(ownTeam, logger, commandManager);
            }
            else
            {
                logger.Log($"{Name} ({Team.TeamName}) пропускает лечение в этот ход.");
            }
            HasUsedSpecial = true; // Mark as used for this turn's special phase
        }

        private void TryHeal(Team ownTeam, ILogger logger, CommandManager commandManager)
        {
            int myIndex = ownTeam.Fighters.IndexOf(this);
            if (myIndex < 0) return;

            // Find the most wounded ally within range (excluding self)
            ICanBeHealed target = ownTeam.Fighters
                .Where(u => u != this && u is ICanBeHealed ch && ch.Health < ch.MaxHealth && Math.Abs(ownTeam.Fighters.IndexOf(u) - myIndex) <= HealRange)
                .Cast<ICanBeHealed>()
                .OrderBy(u => u.Health) // Prioritize lowest health
                .FirstOrDefault();

            if (target != null)
            {
                // Create and execute heal command
                var healCommand = new HealCommand(this, target, HealPower, ownTeam, logger);
                commandManager.ExecuteCommand(healCommand);
            }
            else
            {
                logger.Log($"{Name} ({Team.TeamName}) не нашел раненых союзников в радиусе {HealRange}.");
            }
        }
    }

    // --- Archer ---
    public class Archer : BaseUnit, ICanBeHealed, ICanBeCloned, ISpecialActionArcher
    {
        public int AttackRange { get; private set; }
        public int AttackPower { get; private set; }
        public override int SpecialActionChance => 75; // 75% chance to shoot

        public Archer() : base(nameof(Archer))
        {
            AttackRange = UnitConfig.Stats[nameof(Archer)].Range ?? 3;
            AttackPower = UnitConfig.Stats[nameof(Archer)].Power ?? 15;
        }

        // Cloning constructor
        private Archer(Archer original) : base(original)
        {
            this.AttackRange = original.AttackRange;
            this.AttackPower = original.AttackPower;
        }

        public ICanBeCloned Clone()
        {
            return new Archer(this);
        }

        // Special Action Logic
        public override void PerformSpecialAction(Team ownTeam, Team enemyTeam, ILogger logger, CommandManager commandManager)
        {
            if (HasUsedSpecial || Health <= 0) return;

            if (new Random().Next(100) < SpecialActionChance)
            {
                TrySpecialAttack(enemyTeam, logger, commandManager);
            }
            else
            {
                logger.Log($"{Name} ({Team.TeamName}) пропускает выстрел в этот ход.");
            }
            HasUsedSpecial = true; // Mark as used for this turn's special phase
        }

        private void TrySpecialAttack(Team enemyTeam, ILogger logger, CommandManager commandManager)
        {
            if (!enemyTeam.HasFighters()) return;

            // Target selection: Prioritize enemy front-liner? Or random? Let's do random within range.
            // Find enemies within range from the *perspective of the archer*.
            // This requires knowing the archer's position relative to the front line.
            // Simple approach: Target any enemy unit. More complex: calculate range based on positions.
            // Let's assume for now it can target *any* living enemy.
            List<IUnit> possibleTargets = enemyTeam.Fighters.Where(u => u.Health > 0).ToList();

            if (possibleTargets.Any())
            {
                IUnit target = possibleTargets[new Random().Next(possibleTargets.Count)];

                // Create and execute archer attack command
                var archerAttackCommand = new ArcherAttackCommand(this, target, AttackPower, enemyTeam, logger);
                commandManager.ExecuteCommand(archerAttackCommand);
            }
            else
            {
                logger.Log($"{Name} ({Team.TeamName}) не нашел целей для выстрела.");
            }
        }
    }

    // --- Mage ---
    public class Mage : BaseUnit, ICanBeHealed, ISpecialActionMage // Mages are not clonable themselves by default
    {
        public int CloneRange { get; private set; }
        public override int SpecialActionChance => 10; // Low chance to clone

        public Mage() : base(nameof(Mage))
        {
            CloneRange = UnitConfig.Stats[nameof(Mage)].CloneRange ?? 1;
        }

        // Special Action Logic
        public override void PerformSpecialAction(Team ownTeam, Team enemyTeam, ILogger logger, CommandManager commandManager)
        {
            if (HasUsedSpecial || Health <= 0) return;

            if (new Random().Next(100) < SpecialActionChance)
            {
                TryClone(ownTeam, logger, commandManager);
            }
            else
            {
                // Only log success/failure inside TryClone
                // logger.Log($"{Name} ({Team.TeamName}) не решился на клонирование.");
            }
            HasUsedSpecial = true; // Mark as used for this turn's special phase
        }

        private void TryClone(Team ownTeam, ILogger logger, CommandManager commandManager)
        {
            int myIndex = ownTeam.Fighters.IndexOf(this);
            if (myIndex < 0) return;

            // Find clonable allies within range (excluding self)
            var possibleTargets = ownTeam.Fighters
                .Where((u, index) => u != this && u is ICanBeCloned cloneable && cloneable.Health > 0 && Math.Abs(index - myIndex) <= CloneRange)
                .Cast<ICanBeCloned>()
                .ToList();

            if (!possibleTargets.Any())
            {
                logger.Log($"{Name} ({Team.TeamName}) не нашел подходящих целей для клонирования рядом!");
                return;
            }

            // Clone a random eligible target
            var targetToClone = possibleTargets[new Random().Next(possibleTargets.Count)];
            int targetIndex = ownTeam.Fighters.IndexOf(targetToClone as IUnit); // Find index of the original

            // Create and execute clone command
            // Insert clone *behind* the mage (or original? behind mage seems safer)
            int insertPosition = myIndex + 1;
            var cloneCommand = new CloneCommand(this, targetToClone, ownTeam, insertPosition, logger);
            commandManager.ExecuteCommand(cloneCommand);
        }
    }
}