using UnityEngine;

public class LogicDOT : StatusLogic
{
    public override void OnTurnStart(Combatant owner, StatusEffect effect)
    {
        if (effect.remainingHits > 0)
        {
            //  Twardy limit stacków do obra¿eñ (max 10) ---
            // Gra bierze mniejsz¹ z dwóch liczb: faktyczne stacki albo 10.
            int effectiveStacks = Mathf.Min(effect.remainingHits, 10);

            // Obra¿enia = Baza (np. 10% hita) * efektywne stacki (max 10)
            int damage = effect.value * effectiveStacks;

            owner.ShowFloatingText(effect.effectName + "!", DamagePopup.PopupType.Miss);
            owner.TakeDamage(damage, false, "", true, SkillCategory.MeleePhysical);

            // Zaka¿enie s³abnie > Odejmujemy jeden stack.
            effect.remainingHits--;

            effect.duration = effect.remainingHits;
        }
    }
    public override bool IsExpired(StatusEffect effect)
    {
        return effect.remainingHits <= 0;
    }
}