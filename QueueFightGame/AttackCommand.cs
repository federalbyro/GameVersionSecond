// AttackCommand.cs
using QueueFightGame;
using System;

public class AttackCommand : IGameCommand
{
    private IUnit attacker;
    private IUnit defender;
    private float initialDefenderHealth;
    private Team attackingTeam;
    private Team defendingTeam;

    public AttackCommand(IUnit attacker, IUnit defender, Team attackingTeam, Team defendingTeam)
    {
        this.attacker = attacker;
        this.defender = defender;
        this.attackingTeam = attackingTeam;
        this.defendingTeam = defendingTeam;
        this.initialDefenderHealth = defender.Health;
    }

    public void Execute()
    {
        // Сохраняем состояние перед атакой
        initialDefenderHealth = defender.Health;

        // Выполняем атаку
        attacker.Attack(defender);

        // Проверяем, был ли убит защитник
        if (defender.Health <= 0)
        {
            defendingTeam.RemoveFighter();
        }
    }

    public void Undo()
    {
        // Восстанавливаем здоровье защитника
        defender.Health = initialDefenderHealth;

        // Если защитник был убит, возвращаем его в команду
        if (initialDefenderHealth > 0 && !defendingTeam.Fighters.Contains(defender))
        {
            defendingTeam.Fighters.Insert(0, defender);
        }
    }
}

// HealCommand.cs
public class HealCommand : IGameCommand
{
    private Healer healer;
    private ICanBeHealed target;
    private float healAmount;
    private Team team;

    public HealCommand(Healer healer, ICanBeHealed target, float healAmount, Team team)
    {
        this.healer = healer;
        this.target = target;
        this.healAmount = healAmount;
        this.team = team;
    }

    public void Execute()
    {
        target.Health = Math.Min(100, target.Health + healAmount);
    }

    public void Undo()
    {
        target.Health -= healAmount;
    }
}

// CloneCommand.cs
public class CloneCommand : IGameCommand
{
    private Mage mage;
    private ICanBeCloned original;
    private IUnit clone;
    private Team team;
    private int position;

    public CloneCommand(Mage mage, ICanBeCloned original, Team team, int position)
    {
        this.mage = mage;
        this.original = original;
        this.team = team;
        this.position = position;
    }

    public void Execute()
    {
        clone = original.Clone() as IUnit;
        team.Fighters.Insert(position + 1, clone);
    }

    public void Undo()
    {
        team.Fighters.Remove(clone);
    }
}