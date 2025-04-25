using System;
using System.Collections.Generic;

namespace QueueFightGame
{
    public class AttackCommand : IGameCommand
    {
        private readonly IUnit _attacker;
        private readonly IUnit _defender;
        private readonly Team _attackingTeam;
        private readonly Team _defendingTeam;
        private readonly ILogger _logger;
        private readonly CommandManager _commandManager; // To potentially chain special ability commands

        // State for Undo
        private float _initialDefenderHealth;
        private float _initialAttackerHealth; // If attack has recoil or side effects
        private bool _defenderWasAlive;
        private int _defenderOriginalIndex = -1; // To put back in correct place if killed


        public AttackCommand(IUnit attacker, IUnit defender, Team attackingTeam, Team defendingTeam, ILogger logger, CommandManager commandManager)
        {
            _attacker = attacker;
            _defender = defender;
            _attackingTeam = attackingTeam;
            _defendingTeam = defendingTeam;
            _logger = logger;
            _commandManager = commandManager; // Needed for post-attack checks/actions

            // Store initial state for Undo
            _initialDefenderHealth = _defender.Health;
            _initialAttackerHealth = _attacker.Health; // Store attacker health too
            _defenderWasAlive = _defender.Health > 0;
            if (_defenderWasAlive)
            {
                _defenderOriginalIndex = _defendingTeam.Fighters.IndexOf(_defender);
            }
        }

        public void Execute()
        {
            // Restore state in case this is a Redo
            _defender.Health = _initialDefenderHealth;
            _attacker.Health = _initialAttackerHealth;
            if (_defenderWasAlive && !_defendingTeam.Fighters.Contains(_defender))
            {
                // If defender was killed and removed, put back before re-executing attack
                if (_defenderOriginalIndex != -1)
                    _defendingTeam.AddFighterAt(_defenderOriginalIndex, _defender);
                else // Failsafe: add to end
                    _defendingTeam.AddFighterAt(_defendingTeam.Fighters.Count, _defender);
            }

            // Perform the attack
            _attacker.Attack(_defender, _logger); // Unit handles logging the attack details

            // Check for defender death AFTER attack
            if (_defender.Health <= 0 && _defenderWasAlive)
            {
                // Log death (Team.RemoveDeadFighters will do this, but maybe log here too?)
                // _logger.Log($"{_defender.Name} ({_defendingTeam.TeamName}) погибает от атаки {_attacker.Name}!");
                // Team's responsibility to remove the dead unit in its cleanup phase
                // For Undo, we need to know it was removed. The check in Undo handles this.
            }

            // Check for attacker death (e.g., thorns, reflect damage - implement if needed)
            if (_attacker.Health <= 0)
            {
                // Handle attacker death similarly
            }
        }

        public void Undo()
        {
            // Restore health
            _defender.Health = _initialDefenderHealth;
            _attacker.Health = _initialAttackerHealth;

            // If the defender was killed by this attack and removed from the team list
            if (_defenderWasAlive && _defender.Health > 0 && !_defendingTeam.Fighters.Contains(_defender))
            {
                // Put the defender back into the team at their original position
                if (_defenderOriginalIndex != -1)
                    _defendingTeam.AddFighterAt(_defenderOriginalIndex, _defender);
                else // Failsafe if index wasn't stored correctly
                    _defendingTeam.AddFighterAt(0, _defender); // Add to front as fallback

                _logger.Log($"{_defender.Name} возвращен в бой (Undo).");
            }

            // If attacker died and was removed, restore similarly (if attacker death implemented)
        }
    }
}