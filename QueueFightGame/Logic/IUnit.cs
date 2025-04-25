namespace QueueFightGame
{
    // Keep existing IUnit interface
    public interface IUnit
    {
        string Name { get; }
        int ID { get; } // Unique ID per instance might be useful
        float Health { get; set; }
        float MaxHealth { get; } // Added for UI and healing caps
        float Protection { get; }
        float Damage { get; }
        float Cost { get; }
        string Description { get; }
        string IconPath { get; } // Added for UI
        Team Team { get; set; }

        void Attack(IUnit target, ILogger logger); // Pass logger
    }

    // Keep existing IWall interface (though WallAdapter implements IUnit)
    public interface IWall
    {
        string Name { get; }
        float Health { get; set; }
        float MaxHealth { get; }
        float Protection { get; }
    }
}