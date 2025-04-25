using System;

namespace QueueFightGame
{
    public class HealCommand : IGameCommand
    {
        private readonly Healer _healer;
        private readonly ICanBeHealed _target;
        private readonly int _maxHealAmount; // Max potential heal (e.g., Healer's Power)
        private readonly Team _team;
        private readonly ILogger _logger;

        // State for Undo
        private float _initialTargetHealth;
        private float _actualHealAmount;

        public HealCommand(Healer healer, ICanBeHealed target, int maxHealAmount, Team team, ILogger logger)
        {
            _healer = healer;
            _target = target;
            _maxHealAmount = maxHealAmount;
            _team = team;
            _logger = logger;

            _initialTargetHealth = _target.Health;
        }

        public void Execute()
        {
            // Restore state for Redo
            _target.Health = _initialTargetHealth;

            // Calculate actual heal amount based on random power and health cap
            int healRoll = new Random().Next(0, _maxHealAmount + 1); // Heal 0 to Power
            _actualHealAmount = Math.Min(healRoll, _target.MaxHealth - _target.Health); // Don't overheal

            if (_actualHealAmount > 0)
            {
                _target.Health += _actualHealAmount;
                _logger.Log($"{_healer.Name} ({_team.TeamName}) лечит {_target.Name} на {_actualHealAmount:F1} HP. Текущее здоровье: {_target.Health:F1}/{_target.MaxHealth:F1}");
            }
            else
            {
                _logger.Log($"{_healer.Name} ({_team.TeamName}) пытается лечить {_target.Name}, но не может (цель здорова или не повезло с лечением).");
                _actualHealAmount = 0; // Ensure it's 0 if no heal happened
            }
        }

        public void Undo()
        {
            // Simply subtract the actual amount healed
            if (_actualHealAmount > 0)
            {
                _target.Health -= _actualHealAmount;
            }
            // Log handled by CommandManager
        }
    }
}