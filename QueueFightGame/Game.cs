using QueueFightGame;
using System;

public class Game
{
    private GameManager Manager;

    public Game()
    {
        Manager = new GameManager();
    }

    public void Play()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("1 - Начать игру");
            Console.WriteLine("2 - Отменить ход (Undo)");
            Console.WriteLine("3 - Повторить ход (Redo)");
            Console.WriteLine("4 - Выход");

            var key = Console.ReadKey().Key;

            switch (key)
            {
                case ConsoleKey.D1:
                    Manager.Battle();
                    break;
                case ConsoleKey.D2:
                    Manager.Undo();
                    break;
                case ConsoleKey.D3:
                    Manager.Redo();
                    break;
                case ConsoleKey.D4:
                    return;
            }
        }
    }
}