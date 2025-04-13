using System;
using System.Collections.Generic;
using System.Linq;

namespace QueueFightGame
{
    public class GameManager
    {
        private Team redTeam;
        private Team blueTeam;

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
            BaseWall wall1 = new StoneWall();
            BaseWall wall2 = new StoneWall();
            WallAdapter wallAdapter1 = new WallAdapter(wall1, 13);
            WallAdapter wallAdapter2 = new WallAdapter(wall2, 14);
            BaseUnit mage1 = new Mage("redMage", 15);
            BaseUnit mage2 = new Mage("BlueMage", 16);

            redTeam.AddFighter(wallAdapter1);
            redTeam.AddFighter(archer4);
            redTeam.AddFighter(archer1);
            redTeam.AddFighter(strongFighter2);
            redTeam.AddFighter(weakFighter1);
            redTeam.AddFighter(healer1);
            redTeam.AddFighter(mage1);

            Console.WriteLine($"Money RedTeam {redTeam.Money}");

            Console.WriteLine("---");

            blueTeam.AddFighter(wallAdapter2);
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

        public void Battle()
        {
            Team attackingTeam = RandomStartAttack();
            Team defendingTeam = attackingTeam == redTeam ? blueTeam : redTeam;
            int round = 1;

            while (redTeam.HasFighters() && blueTeam.HasFighters())
            {
                Console.WriteLine("\nНажмите любую клавишу, чтобы продолжить...");
                Console.ReadKey();

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

                Console.WriteLine($"\n{attacker.Name} (HP: {attacker.Health}) атакует {defender.Name} (HP: {defender.Health})");
                attacker.Attack(defender);

                ProcessSpecialAbilities(attackingTeam, defender);

                if (defender.Health <= 0)
                {
                    Console.WriteLine($"\n{defender.Name} пал в бою!");
                    defendingTeam.RemoveFighter();
                }
                (attackingTeam, defendingTeam) = (defendingTeam, attackingTeam);
            }

            if (!redTeam.HasFighters() || !blueTeam.HasFighters())
            {
                Console.WriteLine(redTeam.HasFighters()
                    ? "\nКрасная команда победила!"
                    : "\nСиняя команда победила!");
            }
            else
            {
                Console.WriteLine("\nБой завершен по достижению максимального количества раундов!");
            }
        }

        private void ProcessSpecialAbilities(Team team, IUnit target)
        {
            // Создаем копию списка для безопасной итерации
            var fighters = team.Fighters.Skip(1).ToList();

            foreach (var unit in fighters)
            {
                try
                {
                    if (unit is Archer archer)
                    {
                        archer.DoSpecialAttack(target, team);
                    }
                    else if (unit is Healer healer)
                    {
                        healer.DoHeal(team);
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
                int countBefore = team.Fighters.Count;
                mage.DoClone(team);

                if (team.Fighters.Count == countBefore)
                {
                    Console.WriteLine($"{mage.Name} не смог создать клона!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{mage.Name} провалил заклинание клонирования: {ex.Message}");
            }
        }
    }
}