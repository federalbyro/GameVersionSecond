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

        public void ExecuteCommand(IGameCommand command)
        {
            command.Execute();
            undoStack.Push(command);
            redoStack.Clear();
        }

        public void Undo()
        {
            if (undoStack.Count > 0)
            {
                var command = undoStack.Pop();
                command.Undo();
                redoStack.Push(command);
            }
        }

        public void Redo()
        {
            if (redoStack.Count > 0)
            {
                var command = redoStack.Pop();
                command.Execute();
                undoStack.Push(command);
            }
        }

        public bool CanUndo => undoStack.Count > 0;
        public bool CanRedo => redoStack.Count > 0;
    }
}