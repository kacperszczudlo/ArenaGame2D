using UnityEngine;

public class LogicBlessing : StatusLogic
{
    public override int OnTakeDamage(Combatant owner, StatusEffect effect, int incomingDamage, bool isDot, SkillCategory category)
    {
        // Unikamy ciosów i magii, ale trucizny we krwi nie da siê unikn¹æ!
        if (isDot) return incomingDamage;

        if (effect.remainingHits > 0 && incomingDamage > 0)
        {
            effect.remainingHits--;
            owner.ShowFloatingText("Cudowny Unik!", DamagePopup.PopupType.Miss);
            if (effect.icon != null) owner.PlaySkillEffect(effect.icon);
            return 0;
        }
        return incomingDamage;
    }

    public override bool IsExpired(StatusEffect effect)
    {
        return effect.duration <= 0;
    }
}