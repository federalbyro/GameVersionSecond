using System;
using System.Collections.Generic;
using System.Linq;

namespace QueueFightGame
{
    public class GameManager
    {
        public Team RedTeam { get; private set; }
        public Team BlueTeam { get; private set; }
        public Team CurrentAttacker { get; private set; }
        public Team CurrentDefender { get; private set; }
        public GameState CurrentState { get; private set; }
        public int Round { get; private set; }

        private readonly CommandManager _commandManager;
        public CommandManager CommandManager => _commandManager;
        private readonly ILogger _logger;
        private readonly Random _random = new Random();

        // Events for UI updates
        public event EventHandler<GameStateChangedEventArgs> GameStateChanged;
        public event EventHandler<LogEventArgs> LogGenerated;
        public event EventHandler<GameOverEventArgs> GameOver;


        public GameManager(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commandManager = new CommandManager(_logger); // Pass logger to CommandManager too
            CurrentState = GameState.NotStarted;
        }

        public void StartGame(Team redTeam, Team blueTeam)
        {
            RedTeam = redTeam ?? throw new ArgumentNullException(nameof(redTeam));
            BlueTeam = blueTeam ?? throw new ArgumentNullException(nameof(blueTeam));

            RedTeam.ResetUnitsForNewBattle();
            BlueTeam.ResetUnitsForNewBattle();
            _logger.ClearLog();
            // НЕ пересоздаем CommandManager, а очищаем его историю
            _commandManager.ClearHistory();

            CurrentAttacker = _random.Next(2) == 0 ? RedTeam : BlueTeam;
            CurrentDefender = CurrentAttacker == RedTeam ? BlueTeam : RedTeam;

            Round = 1;
            // ВАЖНО: Устанавливаем состояние ОЖИДАНИЯ, а не TurnInProgress сразу
            CurrentState = GameState.WaitingForPlayer;
            Log($"--- Игра началась! ---");
            Log($"Команда {RedTeam.TeamName} состав: {string.Join(", ", RedTeam.Fighters.Select(f => f.Name))}");
            Log($"Команда {BlueTeam.TeamName} состав: {string.Join(", ", BlueTeam.Fighters.Select(f => f.Name))}");
            Log($"Первый ход за командой: {CurrentAttacker.TeamName}. Нажмите 'Следующий Ход'."); // Подсказка игроку

            OnGameStateChanged(); // Отправляем начальное состояние UI
        }

        public void RequestNextTurn()
        {
            if (CurrentState != GameState.TurnInProgress && CurrentState != GameState.WaitingForPlayer) // Allow starting turn
            {
                Log("Невозможно начать ход, игра не идет или завершена.");
                return;
            }

            CurrentState = GameState.TurnInProgress; // Lock state during processing

            Log($"\n--- Раунд {Round} ---");
            Log($"Ходит команда: {CurrentAttacker.TeamName}");

            // --- Phase 1: Special Abilities ---
            Log("--- Фаза способностей ---");
            ProcessSpecialAbilities(CurrentAttacker); // Attacker's specials first? Or both simultaneously? Let's do attacker then defender.
            ProcessSpecialAbilities(CurrentDefender);
            // Reset special usage flags for next turn cycle
            ResetSpecialAbilityFlags(CurrentAttacker);
            ResetSpecialAbilityFlags(CurrentDefender);
            // Check for deaths after specials
            CheckForDeaths();
            if (CheckWinCondition()) return; // Check if specials caused game over

            // --- Phase 2: Main Attack ---
            Log($"--- Фаза атаки ({CurrentAttacker.TeamName}) ---");
            IUnit attackerUnit = CurrentAttacker.GetNextFighter();
            IUnit defenderUnit = CurrentDefender.GetNextFighter();

            if (attackerUnit != null && defenderUnit != null)
            {
                var attackCommand = new AttackCommand(attackerUnit, defenderUnit, CurrentAttacker, CurrentDefender, _logger, _commandManager);
                _commandManager.ExecuteCommand(attackCommand);
            }
            else
            {
                Log("Невозможно атаковать: отсутствует один из бойцов на передовой.");
            }

            // --- Phase 3: Cleanup & State Change ---
            CheckForDeaths(); // Remove units killed by the main attack
            if (CheckWinCondition()) return; // Check for game over after attack

            // Switch turns
            (CurrentAttacker, CurrentDefender) = (CurrentDefender, CurrentAttacker);
            Round++; // Increment round after a full cycle (both teams acted or tried to)

            CurrentState = GameState.WaitingForPlayer; // Ready for next input
            Log($"Ход завершен. Ожидание следующего хода (Раунд {Round}).");
            OnGameStateChanged();
        }

        private void ProcessSpecialAbilities(Team team)
        {
            Log($"-- Способности команды {team.TeamName} --");
            var livingFighters = team.GetLivingFighters(); // Process only living units

            foreach (var unit in livingFighters)
            {
                if (unit is ISpecialActionUnit specialUnit && !specialUnit.HasUsedSpecial)
                {
                    try
                    {
                        // PerformSpecialAction handles its own probability check now
                        specialUnit.PerformSpecialAction(team, team == RedTeam ? BlueTeam : RedTeam, _logger, _commandManager);
                    }
                    catch (Exception ex)
                    {
                        Log($"ОШИБКА способности у {unit.Name}: {ex.Message}");
                    }
                }
            }
        }

        private void ResetSpecialAbilityFlags(Team team)
        {
            foreach (var unit in team.Fighters.OfType<ISpecialActionUnit>())
            {
                unit.HasUsedSpecial = false;
            }
        }

        private void CheckForDeaths()
        {
            RedTeam.RemoveDeadFighters(_logger);
            BlueTeam.RemoveDeadFighters(_logger);
        }

        private bool CheckWinCondition()
        {
            bool redLost = !RedTeam.HasFighters();
            bool blueLost = !BlueTeam.HasFighters();

            if (redLost && blueLost)
            {
                CurrentState = GameState.GameOver;
                Log("\n--- Игра окончена: НИЧЬЯ! ---");
                OnGameOver(null); // Draw
                return true;
            }
            if (redLost)
            {
                CurrentState = GameState.GameOver;
                Log($"\n--- Игра окончена: Победила команда {BlueTeam.TeamName}! ---");
                OnGameOver(BlueTeam); // Blue wins
                return true;
            }
            if (blueLost)
            {
                CurrentState = GameState.GameOver;
                Log($"\n--- Игра окончена: Победила команда {RedTeam.TeamName}! ---");
                OnGameOver(RedTeam); // Red wins
                return true;
            }
            return false;
        }

        public void RequestUndoTurn()
        {
            if (CurrentState == GameState.GameOver)
            {
                Log("Нельзя отменить ход: игра завершена.");
                return;
            }
            if (CurrentState == GameState.TurnInProgress)
            {
                Log("Нельзя отменить ход во время его обработки.");
                return;
            }

            if (_commandManager.CanUndo)
            {
                // Before undoing, switch back the CurrentAttacker/Defender if the last action completed a turn cycle
                // This logic depends on when Undo is allowed. If allowed mid-turn, it's complex.
                // Assume Undo reverts the *last completed action* (attack or special ability).
                _commandManager.Undo();

                // After undo, the game state might be complex. We need to signal UI to refresh.
                // Re-evaluating attacker/defender and round might be needed if a full turn was undone.
                // For simplicity, let Undo just revert the last command(s) and trigger UI refresh.
                // The state might become slightly inconsistent until next turn request corrects it.
                CurrentState = GameState.WaitingForPlayer; // Allow next action
                CheckForDeaths(); // Ensure no revived units are immediately removed
                Log("Последнее действие отменено.");
                OnGameStateChanged(); // Notify UI to refresh display
            }
            else
            {
                Log("Нет действий для отмены.");
            }
        }

        // --- Event Invokers ---
        protected virtual void OnGameStateChanged()
        {
            GameStateChanged?.Invoke(this, new GameStateChangedEventArgs(RedTeam, BlueTeam, CurrentState, GetLogHistory()));
        }

        protected virtual void OnGameOver(Team winner)
        {
            GameOver?.Invoke(this, new GameOverEventArgs(winner));
            OnGameStateChanged(); // Send final state
        }

        // Helper to raise log event (and maybe log internally too)
        private void Log(string message)
        {
            _logger.Log(message); // Log to internal logger (MemoryLogger)
            LogGenerated?.Invoke(this, new LogEventArgs(message)); // Notify subscribers
        }

        public List<string> GetLogHistory()
        {
            return _logger.GetLogHistory();
        }
    }

    // --- Enums and Event Args ---
    public enum GameState
    {
        NotStarted,
        WaitingForPlayer, // Waiting for "Next Turn" or "Undo"
        TurnInProgress,   // Processing turn logic
        GameOver
    }

    public class GameStateChangedEventArgs : EventArgs
    {
        public Team RedTeamSnapshot { get; } // Consider sending copies/snapshots if needed
        public Team BlueTeamSnapshot { get; }
        public GameState CurrentState { get; }
        public List<string> LogMessages { get; }

        public GameStateChangedEventArgs(Team red, Team blue, GameState state, List<string> logs)
        {
            RedTeamSnapshot = red; // For now, pass reference. UI should be careful.
            BlueTeamSnapshot = blue;
            CurrentState = state;
            LogMessages = new List<string>(logs); // Copy of logs
        }
    }

    public class LogEventArgs : EventArgs
    {
        public string Message { get; }
        public LogEventArgs(string message) { Message = message; }
    }

    public class GameOverEventArgs : EventArgs
    {
        public Team WinningTeam { get; } // null for a draw
        public GameOverEventArgs(Team winner) { WinningTeam = winner; }
    }
}