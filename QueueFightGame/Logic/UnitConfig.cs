using System.Collections.Generic;

namespace QueueFightGame
{
    public static class UnitConfig
    {
        public class UnitData
        {
            public string TypeName { get; set; }
            public string DisplayName { get; set; }
            public float Health { get; set; }
            public float Protection { get; set; } 
            public float Damage { get; set; }
            public float Cost { get; set; }
            public string Description { get; set; }
            public string IconPath { get; set; } 
            public int? Range { get; set; }
            public int? Power { get; set; }
            public int? CloneRange { get; set; }
            public int? BuffRange { get; set; }
        }

        public static readonly Dictionary<string, UnitData> Stats = new Dictionary<string, UnitData>
        {
            {
                nameof(WeakFighter), new UnitData {
                    TypeName = nameof(WeakFighter), DisplayName = "Оруженосец", Health = 80f, Protection = 0.8f, Damage = 35, Cost = 15,
                    Description = "Слабый боец ближнего боя. Может баффать Рыцаря.", IconPath = "Resources/light_weight.PNG", BuffRange = 1
                }
            },
            {
                nameof(StrongFighter), new UnitData {
                    TypeName = nameof(StrongFighter), DisplayName = "Рыцарь", Health = 120f, Protection = 0.6f, Damage = 50, Cost = 30,
                    Description = "Сильный боец ближнего боя. Получает бонусы от Оруженосца.", IconPath = "Resources/hard_fihter.PNG"
                }
            },
            {
                nameof(Healer), new UnitData {
                    TypeName = nameof(Healer), DisplayName = "Лекарь", Health = 70f, Protection = 0.9f, Damage = 20, Cost = 25,
                    Description = "Лечит союзников на расстоянии.", IconPath = "Resources/heal.PNG", Range = 1, Power = 20 
                }
            },
            {
                nameof(Archer), new UnitData {
                    TypeName = nameof(Archer), DisplayName = "Лучник", Health = 60f, Protection = 0.9f, Damage = 27, Cost = 20,
                    Description = "Атакует врагов на расстоянии.", IconPath = "Resources/archier.PNG", Range = 3, Power = 15
                }
            },
            {
                nameof(Mage), new UnitData {
                    TypeName = nameof(Mage), DisplayName = "Маг", Health = 75f, Protection = 0.85f, Damage = 20, Cost = 35,
                    Description = "Может клонировать союзников.", IconPath = "Resources/mag.PNG", CloneRange = 1 
                }
            },
             {
                nameof(WallAdapter), new UnitData { 
                    TypeName = nameof(WallAdapter), DisplayName = "Гуляй-город", Health = 200f, Protection = 0.3f, Damage = 0, Cost = 30,
                    Description = "Прочная стена, не может атаковать.", IconPath = "Resources/wall.PNG"
                }
            }
        };
    }
}