namespace QueueFightGame
{
    // Keep existing ICanBeHealed, ICanBeCloned, ICanBeBuff, BuffType
    public interface ICanBeHealed
    {
        string Name { get; }
        float Health { get; set; }
        float MaxHealth { get; } // Added
    }

    public interface ICanBeCloned : IUnit
    {
        ICanBeCloned Clone(); // Returns the specific clonable type
    }

    public interface ICanBeBuff
    {
        BuffType BuffType { get; }
        float DamageMultiplier { get; }
        float GetModifiedProtection(IUnit attacker); // Changed from ShouldBlockDamage
        void ApplyBuffEffect(StrongFighter fighter); // Method to apply visual/other effects if needed
        void RemoveBuffEffect(StrongFighter fighter); // Method to remove visual/other effects
    }

    public enum BuffType
    {
        None,
        Spear, // Increased damage, one use
        Horse, // Increased damage, first strike advantage (maybe block first hit?)
        Shield,// Increased protection
        Helmet // Protection against ranged
    }
}