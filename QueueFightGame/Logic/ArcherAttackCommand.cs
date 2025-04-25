using System;

namespace QueueFightGame
{
    public class ArcherAttackCommand : IGameCommand
    {
        private readonly Archer _archer;
        private readonly IUnit _target;
        private readonly int _power;
        private readonly Team _targetTeam;
        private readonly ILogger _logger;

        // State for Undo
        private float _initialTargetHealth;
        private float _damageDealt; // Store actual damage dealt

        public ArcherAttackCommand(Archer archer, IUnit target, int power, Team targetTeam, ILogger logger)
        {
            _archer = archer;
            _target = target;
            _power = power;
            _targetTeam = targetTeam;
            _logger = logger;

            _initialTargetHealth = _target.Health;
            _damageDealt = 0; // Initialize
        }

        public void Execute()
        {
            // Restore state for Redo
            _target.Health = _initialTargetHealth;
            _damageDealt = 0;

            // Archer accuracy check (e.g., 70% hit chance)
            bool isHit = new Random().Next(100) < 70;

            if (isHit)
            {
                // Calculate damage based on archer power and target protection
                _damageDealt = Math.Max(1, _power * (1.0f - _target.Protection));
                _target.Health -= _damageDealt;
                _logger.Log($"{_archer.Name} ({_archer.Team.TeamName}) стреляет в {_target.Name} ({_targetTeam.TeamName}) и ПОПАДАЕТ, нанося {_damageDealt:F1} урона. Осталось здоровья: {_target.Health:F1}/{_target.MaxHealth:F1}");

                // Check for target death (Team handles removal later)
                if (_target.Health <= 0)
                {
                    // Log is handled by Team.RemoveDeadFighters
                }
            }
            else
            {
                _logger.Log($"{_archer.Name} ({_archer.Team.TeamName}) стреляет в {_target.Name} ({_targetTeam.TeamName}), но ПРОМАХИВАЕТСЯ!");
                _damageDealt = 0; // Ensure 0 if missed
            }
        }

        public void Undo()
        {
            // Restore health only if damage was dealt
            if (_damageDealt > 0)
            {
                _target.Health += _damageDealt; // Add back the damage
            }

            // If target was killed and removed, team needs to be restored by AttackCommand undo or similar mechanism.
            // Simple special attacks often don't handle target revival directly, relying on the main turn undo.
            // However, if this command *could* kill, we might need defender revival logic like in AttackCommand.
            // For now, assume main attack undo handles revival if needed.

            // Log handled by CommandManager
        }
    }
}