using System;

namespace QueueFightGame
{
    public class CloneCommand : IGameCommand
    {
        private readonly Mage _mage;
        private readonly ICanBeCloned _original; // The unit type being cloned
        private readonly Team _team;
        private readonly int _insertPosition;
        private readonly ILogger _logger;

        // State for Undo
        private IUnit _createdClone; // Keep reference to the created clone

        public CloneCommand(Mage mage, ICanBeCloned original, Team team, int insertPosition, ILogger logger)
        {
            _mage = mage;
            _original = original;
            _team = team;
            _insertPosition = insertPosition;
            _logger = logger;
        }

        public void Execute()
        {
            // If Redo, the clone might exist but was removed by Undo.
            // We need to re-create it or re-insert it. Re-creating is simpler.
            _createdClone = _original.Clone() as IUnit; // Create the clone

            if (_createdClone != null)
            {
                // Ensure position is valid
                int actualPosition = Math.Max(0, Math.Min(_insertPosition, _team.Fighters.Count));
                _team.AddFighterAt(actualPosition, _createdClone); // Use AddFighterAt
                _logger.Log($"{_mage.Name} ({_team.TeamName}) успешно клонировал {_original.Name}, создав {_createdClone.Name}!");
            }
            else
            {
                _logger.Log($"{_mage.Name} ({_team.TeamName}) не смог создать клон {_original.Name} (ошибка клонирования).");
            }
        }

        public void Undo()
        {
            if (_createdClone != null && _team.Fighters.Contains(_createdClone))
            {
                _team.RemoveFighter(_createdClone, _logger, false); // Remove the clone, no refund
                // Log handled by CommandManager
            }
            // Set _createdClone = null; ? Probably not needed, Execute will re-create.
        }
    }
}