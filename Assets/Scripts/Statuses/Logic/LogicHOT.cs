using UnityEngine;

public class LogicHOT : StatusLogic
{
    public override void OnTurnStart(Combatant owner, StatusEffect effect)
    {
        owner.Heal(effect.value, "Regeneracja");
    }

    public override bool IsExpired(StatusEffect effect)
    {
        return effect.duration <= 0;
    }
}