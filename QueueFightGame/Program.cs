using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueFightGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            WeakFighter weakFighter = new WeakFighter();
            StrongFighter strongFighter = new StrongFighter();
            Archer archer = new Archer();
            Healer healer = new Healer();

            strongFighter.Attack(weakFighter);
            strongFighter.Attack(weakFighter);
            strongFighter.Attack(weakFighter);

            Menu();
        }
        static void Menu()
        {
            bool key = true;
            while (key)
            {
                Console.WriteLine("Write number");
                int inputNumber = Convert.ToInt32(Console.ReadLine());
                switch (inputNumber)
                {
                    case 1:
                        Console.WriteLine("1. Поместить в команду синих");
                        break;
                    case 2:
                        Console.WriteLine("2. Поместить в команду красных");
                        break;
                    case 3:
                        Console.WriteLine("3. Атака команды красных");
                        break;
                    case 4:
                        Console.WriteLine("4. Атака команды синих");
                        break;
                    case 5:
                        Console.WriteLine("5. Выход");
                        break;
                    default:
                        key = false;
                        break;
                }


            }

        }
    }

}
