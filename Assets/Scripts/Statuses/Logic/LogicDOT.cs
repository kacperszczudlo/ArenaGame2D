using UnityEngine;

public class LogicDOT : StatusLogic
{
    public override void OnTurnStart(Combatant owner, StatusEffect effect)
    {
        owner.TakeDamage(effect.value, false, "Status");
    }

    public override bool IsExpired(StatusEffect effect)
    {
        return effect.duration <= 0;
    }
}