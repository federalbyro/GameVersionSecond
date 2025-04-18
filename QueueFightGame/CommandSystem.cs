using System;
using System.Collections.Generic;

namespace QueueFightGame
{
    public interface IGameCommand
    {
        void Execute();
        void Undo();
    }

    public class CommandManager
    {
        private Stack<IGameCommand> undoStack = new Stack<IGameCommand>();
        private Stack<IGameCommand> redoStack = new Stack<IGameCommand>();
        private const int MAX_UNDO_LEVELS = 20;

        public void ExecuteCommand(IGameCommand command)
        {
            command.Execute();
            undoStack.Push(command);

            if (undoStack.Count > MAX_UNDO_LEVELS)
            {
                // Удаляем самую старую команду
                var tempStack = new Stack<IGameCommand>();
                for (int i = 0; i < undoStack.Count - 1; i++)
                {
                    tempStack.Push(undoStack.Pop());
                }
                undoStack.Pop(); // Удаляем самую старую команду
                while (tempStack.Count > 0)
                {
                    undoStack.Push(tempStack.Pop());
                }
            }

            redoStack.Clear();
        }

        public void Undo()
        {
            if (undoStack.Count > 0)
            {
                var command = undoStack.Pop();
                command.Undo();
                redoStack.Push(command);
                Console.WriteLine("Ход отменен");
            }
            else
            {
                Console.WriteLine("Нет ходов для отмены");
            }
        }

        public void Redo()
        {
            if (redoStack.Count > 0)
            {
                var command = redoStack.Pop();
                command.Execute();
                undoStack.Push(command);
                Console.WriteLine("Ход повторен");
            }
            else
            {
                Console.WriteLine("Нет ходов для повторения");
            }
        }

        public bool CanUndo => undoStack.Count > 0;
        public bool CanRedo => redoStack.Count > 0;
        public int UndoCount => undoStack.Count;
        public int RedoCount => redoStack.Count;
    }
}