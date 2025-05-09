using System;

namespace QueueFightGame
{
    public abstract class BaseUnit : IUnit
    {
        private static int _nextId = 1;

        public string Name { get; protected set; }
        public int ID { get; private set; }
        public float Health { get; set; }
        public float MaxHealth { get; protected set; }
        public float Protection { get; protected set; }
        public float Damage { get; protected set; }
        public float Cost { get; private set; }
        public string Description { get; private set; }
        public string IconPath { get; private set; }
        public Team Team { get; set; }
        public override string ToString() => $"{Name}#{ID}";
        protected BaseUnit(string typeName) // Constructor uses UnitConfig
        {
            if (!UnitConfig.Stats.TryGetValue(typeName, out var data))
            {
                throw new ArgumentException($"Configuration not found for unit type: {typeName}");
            }

            ID = _nextId++;
            Name = data.DisplayName; // Use DisplayName for game display
            MaxHealth = data.Health;
            Health = data.Health;
            Protection = data.Protection;
            Damage = data.Damage;
            Cost = data.Cost;
            Description = data.Description;
            IconPath = data.IconPath;
            Team = null;
        }

        // Overloaded constructor for cloning to preserve ID/state if needed, but usually new ID is better
        protected BaseUnit(BaseUnit original, string cloneSuffix = "_clone")
        {
            ID = _nextId++; // Clones get new IDs
            Name = original.Name + cloneSuffix;
            MaxHealth = original.MaxHealth;
            Health = original.Health * 0.75f; // Clones start with less health? Or full? Decide rule. Let's say full for now.
            Health = original.MaxHealth;
            Protection = original.Protection;
            Damage = original.Damage;
            Cost = original.Cost; // Clones don't cost money to create in battle
            Description = original.Description;
            IconPath = original.IconPath;
            Team = original.Team; // Will be set correctly when added to team
        }


        public virtual void Attack(IUnit target, ILogger logger)
        {
            // Basic attack logic
            float damageDealt = Math.Max(1, this.Damage * (1.0f - target.Protection)); // Ensure at least 1 damage, adjust formula as needed
            target.Health -= damageDealt;
            logger.Log($"{this.Name} ({this.Team.TeamName}) атакует {target.Name} ({target.Team.TeamName}) и наносит {damageDealt:F1} урона. Осталось здоровья у {target.Name}: {target.Health:F1}");
        }

        // Base implementation for non-special units
        public virtual bool HasUsedSpecial { get; set; } = false; // Default implementation
        public virtual int SpecialActionChance => 0; // Default: no special action

        // Base implementation - does nothing unless overridden
        public virtual void PerformSpecialAction(Team ownTeam, Team enemyTeam, ILogger logger, CommandManager commandManager)
        {
            // Do nothing by default
        }
    }

    // --- Wall and Adapter ---
    public abstract class BaseWall : IWall
    {
        public string Name { get; }
        public float Health { get; set; }
        public float MaxHealth { get; }
        public float Protection { get; set; }

        protected BaseWall(string name, float health, float protection)
        {
            Name = name;
            MaxHealth = health;
            Health = health;
            Protection = protection;
        }
    }

    public class StoneWall : BaseWall
    {
        // Uses fixed stats, not from UnitConfig directly unless we add walls there
        public StoneWall() : base("Каменная стена", 200, 0.3f) { }
    }

    // WallAdapter adapts BaseWall to IUnit using data from UnitConfig
    public class WallAdapter : BaseUnit, ICanBeHealed // Make walls healable? Optional.
    {
        private readonly BaseWall _wall; // Keep reference if needed, maybe for specific wall logic

        public WallAdapter() : base(nameof(WallAdapter)) // Uses UnitConfig based on class name
        {
            // If specific wall type needed:
            //_wall = wall ?? throw new ArgumentNullException(nameof(wall));
            // Use wall stats if they differ significantly from UnitConfig
            // MaxHealth = _wall.MaxHealth; Health = _wall.Health; Protection = _wall.Protection;
        }

        // Walls cannot attack
        public override void Attack(IUnit target, ILogger logger)
        {
            logger.Log($"{this.Name} ({this.Team.TeamName}) не может атаковать.");
        }

        // Walls don't have special actions by default
        public override void PerformSpecialAction(Team ownTeam, Team enemyTeam, ILogger logger, CommandManager commandManager) { }
    }


}