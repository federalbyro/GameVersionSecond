using System;
using System.Linq;

namespace QueueFightGame
{
    public class SquireBuffCommand : IGameCommand
    {
        private readonly WeakFighter _squire;
        private readonly StrongFighter _knight;
        private readonly Team _team;
        private readonly ILogger _logger;

        // State for Undo
        private BuffType _appliedBuffType = BuffType.None;
        private ICanBeBuff _previousKnightBuff = null; // Store if knight already had a different buff

        public SquireBuffCommand(WeakFighter squire, StrongFighter knight, Team team, ILogger logger)
        {
            _squire = squire;
            _knight = knight;
            _team = team;
            _logger = logger;
        }

        public void Execute()
        {
            // Restore state for Redo
            if (_appliedBuffType != BuffType.None)
            {
                _squire.MarkBuffApplied(_knight); // Re-mark squire
                                                  // Re-apply the specific buff that was chosen before
                ApplySpecificBuff(_appliedBuffType);
            }
            else // First execution or Redo after failed Execute
            {
                // Check if knight can receive buff (e.g., doesn't have a persistent buff already?)
                // For now, let's assume we can always try to apply. StrongFighter.ApplyBuff handles existing buffs.

                // Choose random buff
                var possibleBuffs = new[] { BuffType.Spear, BuffType.Horse, BuffType.Shield, BuffType.Helmet };
                _appliedBuffType = possibleBuffs[new Random().Next(possibleBuffs.Length)];

                // Store previous buff for Undo, *before* applying new one
                // This requires StrongFighter to expose its current buff instance or type
                // Let's assume StrongFighter.CurrentBuffType exists
                var existingBuffType = _knight.CurrentBuffType;
                if (existingBuffType != BuffType.None)
                {
                    // Ideally store the actual ICanBeBuff instance, but type might be enough for simple restore
                    // _previousKnightBuff = _knight.GetCurrentBuffInstance(); // Needs implementation in StrongFighter
                    _logger.Log($"Предупреждение: {_knight.Name} уже имел бафф {existingBuffType}. Он будет заменен.");
                    // Force remove old buff? Let ApplySpecificBuff handle it.
                }


                if (ApplySpecificBuff(_appliedBuffType))
                {
                    _squire.MarkBuffApplied(_knight); // Link squire and knight
                }
                else
                {
                    _appliedBuffType = BuffType.None; // Mark as failed
                }
            }
        }


        private bool ApplySpecificBuff(BuffType buffType)
        {
            ICanBeBuff buffInstance = null;
            switch (buffType)
            {
                case BuffType.Spear: buffInstance = new SpearBuffDecorator(_knight); break;
                case BuffType.Horse: buffInstance = new HorseBuffDecorator(_knight); break;
                case BuffType.Shield: buffInstance = new ShieldBuffDecorator(_knight); break;
                case BuffType.Helmet: buffInstance = new HelmetBuffDecorator(_knight); break;
                default: return false; // Should not happen
            }

            _knight.ApplyBuff(buffInstance, _logger); // Apply the buff
            _knight.SetSquire(_squire); // Link knight back to squire
            return true;
        }


        public void Undo()
        {
            if (_appliedBuffType != BuffType.None)
            {
                _squire.UnmarkBuffApplied(); // Unmark squire
                _knight.RemoveBuff(_logger); // Remove the buff applied by this command

                // Restore previous buff if one existed? This is complex.
                // If _previousKnightBuff was stored, re-apply it here.
                // if (_previousKnightBuff != null) { _knight.ApplyBuff(_previousKnightBuff, _logger); }

                _appliedBuffType = BuffType.None; // Reset state
            }
            // Log handled by CommandManager
        }
    }
}