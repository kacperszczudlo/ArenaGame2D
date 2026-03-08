using UnityEngine;

public abstract class StatusLogic
{
    // Co status robi na pocz¹tku tury (np. Krwawienie zabiera HP)
    public virtual void OnTurnStart(Combatant owner, StatusEffect effect) { }

    // Co status robi, gdy postaæ dostaje cios (np. Tarcza go redukuje)
    public virtual int OnTakeDamage(Combatant owner, StatusEffect effect, int incomingDamage, bool isDot, SkillCategory category)
    {
        return incomingDamage;
    }

    // Kiedy status ma znikn¹æ z paska?
    public abstract bool IsExpired(StatusEffect effect);
}