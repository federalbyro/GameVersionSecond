using System;
using System.Collections.Generic;
using System.Linq;

namespace QueueFightGame
{

    public interface IGameCommand
    {
        void Execute();
        void Undo();
        // string GetLogMessage(); // Optional: command provides its own log string
    }
    public class CommandManager
    {
        private readonly Stack<IGameCommand> _undoStack = new Stack<IGameCommand>();
        private readonly Stack<IGameCommand> _redoStack = new Stack<IGameCommand>();
        private const int MaxUndoLevels = 20;
        private readonly ILogger _logger;

        public CommandManager(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // ... (ExecuteCommand, Undo, Redo, CanUndo, CanRedo) ...

        // Новый метод для очистки истории
        public void ClearHistory()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            _logger?.Log("История команд очищена."); // Логируем, если нужно
        }

        public void ExecuteCommand(IGameCommand command)
        {
            command.Execute();
            _undoStack.Push(command);

            // Limit undo stack size
            if (_undoStack.Count > MaxUndoLevels)
            {
                var list = _undoStack.ToList();
                list.RemoveAt(0);
                _undoStack.Clear();
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    _undoStack.Push(list[i]);
                }
            }
            _redoStack.Clear();
        }

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        public void Undo()
        {
            if (CanUndo)
            {
                var command = _undoStack.Pop();
                command.Undo();
                _redoStack.Push(command);
                _logger.Log("Действие отменено (Undo).");
            }
            else
            {
                _logger.Log("Нет действий для отмены.");
            }
        }

        public void Redo()
        {
            if (CanRedo)
            {
                var command = _redoStack.Pop();
                command.Execute();
                _undoStack.Push(command);
                _logger.Log("Действие повторено (Redo).");
            }
            else
            {
                _logger.Log("Нет действий для повтора.");
            }
        }
    }
}