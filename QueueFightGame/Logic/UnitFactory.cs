using System;

namespace QueueFightGame
{
    public static class UnitFactory
    {
        public static IUnit CreateUnit(string typeName)
        {
            
            switch (typeName)
            {
                case nameof(WeakFighter):
                    return new WeakFighter();
                case nameof(StrongFighter):
                    return new StrongFighter();
                case nameof(Healer):
                    return new Healer();
                case nameof(Archer):
                    return new Archer();
                case nameof(Mage):
                    return new Mage();
                case nameof(WallAdapter):
                    return new WallAdapter();
                default:
                    throw new ArgumentException($"Unknown unit type name: {typeName}");
            }
        }

        public static UnitConfig.UnitData GetUnitData(string typeName)
        {
            if (UnitConfig.Stats.TryGetValue(typeName, out var data))
            {
                return data;
            }
            throw new ArgumentException($"Configuration not found for unit type: {typeName}");
        }
    }
}