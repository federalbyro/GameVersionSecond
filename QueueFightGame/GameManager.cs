using System;
using System.Collections.Generic;
using System.Linq;

namespace QueueFightGame
{
    public class GameManager
    {
        private Team redTeam;
        private Team blueTeam;
        private CommandManager commandManager = new CommandManager();

        public GameManager()
        {
            redTeam = new Team("Red", 200);
            blueTeam = new Team("Blue", 200);

            CreateFighters();
            Battle();
        }

        public void CreateFighters()
        {
            BaseUnit weakFighter1 = new WeakFighter(1);
            BaseUnit weakFighter2 = new WeakFighter(2);
            BaseUnit strongFighter1 = new StrongFighter(3);
            BaseUnit strongFighter2 = new StrongFighter(4);
            BaseUnit archer1 = new Archer("Red_Archer", 5);
            BaseUnit archer2 = new Archer("Blue_Archer", 6);
            BaseUnit archer3 = new Archer("Blue_Archer", 7);
            BaseUnit archer4 = new Archer("Red_Archer", 8);
            BaseUnit healer1 = new Healer("Red_Healer", 9);
            BaseUnit healer2 = new Healer("Blue_Healer", 10);
            //BaseWall wall1 = new StoneWall();
            //BaseWall wall2 = new StoneWall();
            //WallAdapter wallAdapter1 = new WallAdapter(wall1, 13);
            //WallAdapter wallAdapter2 = new WallAdapter(wall2, 14);
            BaseUnit mage1 = new Mage("redMage", 15);
            BaseUnit mage2 = new Mage("BlueMage", 16);

            //redTeam.AddFighter(wallAdapter1);
            redTeam.AddFighter(archer4);
            redTeam.AddFighter(archer1);
            redTeam.AddFighter(strongFighter2);
            redTeam.AddFighter(weakFighter1);
            redTeam.AddFighter(healer1);
            redTeam.AddFighter(mage1);

            Console.WriteLine($"Money RedTeam {redTeam.Money}");

            Console.WriteLine("---");

            //blueTeam.AddFighter(wallAdapter2);
            blueTeam.AddFighter(archer3);
            blueTeam.AddFighter(weakFighter2);
            blueTeam.AddFighter(strongFighter1);
            blueTeam.AddFighter(archer2);
            blueTeam.AddFighter(healer2);
            blueTeam.AddFighter(mage2);

            Console.WriteLine($"Money BlueTeam {blueTeam.Money}");
        }

        private Team RandomStartAttack()
        {
            Random randomStartAttack = new Random();
            return randomStartAttack.Next(2) == 0 ? redTeam : blueTeam;
        }

        public void Undo()
        {
            commandManager.Undo();
        }

        public void Redo()
        {
            commandManager.Redo();
        }

        public void Battle()
        {
            Team attackingTeam = RandomStartAttack();
            Team defendingTeam = attackingTeam == redTeam ? blueTeam : redTeam;
            int round = 1;

            while (redTeam.HasFighters() && blueTeam.HasFighters())
            {
                Console.WriteLine("\nНажмите:");
                Console.WriteLine("1 - Следующий ход");
                Console.WriteLine("2 - Отменить ход (Undo)");
                Console.WriteLine("3 - Повторить ход (Redo)");

                var key = Console.ReadKey().Key;

                if (key == ConsoleKey.D2 && commandManager.CanUndo)
                {
                    commandManager.Undo();
                    continue;
                }
                else if (key == ConsoleKey.D3 && commandManager.CanRedo)
                {
                    commandManager.Redo();
                    continue;
                }
                else if (key != ConsoleKey.D1)
                {
                    continue;
                }

                redTeam.ShowTeam();
                blueTeam.ShowTeam();

                Console.WriteLine($"\n--- Раунд {round++} ---");
                Console.WriteLine($"Ходит команда: {(attackingTeam == redTeam ? "Красная" : "Синяя")}");

                IUnit attacker = attackingTeam.GetNextFighter();
                IUnit defender = defendingTeam.GetNextFighter();

                if (attacker == null || defender == null)
                {
                    Console.WriteLine("Ошибка: отсутствует один из бойцов!");
                    break;
                }

                float defenderInitialHealth = defender.Health;

                Console.WriteLine($"\n{attacker.Name} (HP: {attacker.Health}) атакует {defender.Name} (HP: {defender.Health})");

                // Создаем команду для атаки
                var attackCommand = new AttackCommand(attacker, defender, attackingTeam, defendingTeam);
                commandManager.ExecuteCommand(attackCommand);

                // Обработка специальных способностей
                ProcessSpecialAbilities(attackingTeam, defender);

                (attackingTeam, defendingTeam) = (defendingTeam, attackingTeam);
            }
        }

        private void ProcessSpecialAbilities(Team team, IUnit target)
        {
            var fighters = team.Fighters.ToList();

            foreach (var unit in fighters)
            {
                try
                {
                    if (unit is ISpecialActionWeakFighter squire && unit.Health > 0)
                    {
                        squire.DoBuff(team);
                    }
                    else if (unit is Archer archer)
                    {
                        // Создаем команду для специальной атаки лучника
                        // (реализация аналогична AttackCommand)
                    }
                    else if (unit is Healer healer)
                    {
                        var healTarget = (ICanBeHealed)team.Fighters
                            .FirstOrDefault(u => u is ICanBeHealed && u.Health < 100);

                        if (healTarget != null)
                        {
                            float healAmount = new Random().Next(0, healer.Power + 1);
                            var healCommand = new HealCommand(healer, healTarget, healAmount, team);
                            commandManager.ExecuteCommand(healCommand);
                        }
                    }
                    else if (unit is Mage mage)
                    {
                        SafeMageClone(mage, team);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при выполнении способности: {ex.Message}");
                }
            }
        }

        private void SafeMageClone(Mage mage, Team team)
        {
            try
            {
                var possibleTargets = team.Fighters
                    .Where(u => u is ICanBeCloned && u != mage)
                    .Cast<ICanBeCloned>()
                    .ToList();

                if (possibleTargets.Any())
                {
                    var target = possibleTargets[new Random().Next(possibleTargets.Count)];
                    int position = team.Fighters.IndexOf(target as IUnit);

                    var cloneCommand = new CloneCommand(mage, target, team, position);
                    commandManager.ExecuteCommand(cloneCommand);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{mage.Name} провалил заклинание клонирования: {ex.Message}");
            }
        }
    }
}