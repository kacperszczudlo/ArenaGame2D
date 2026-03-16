using UnityEngine;

public class LogicDOT : StatusLogic
{
    public override void OnTurnStart(Combatant owner, StatusEffect effect)
    {
        if (effect.remainingHits > 0)
        {
            // --- NOWOŒÆ: Twardy limit stacków do obra¿eñ (max 10) ---
            // Gra bierze mniejsz¹ z dwóch liczb: faktyczne stacki albo 10.
            int effectiveStacks = Mathf.Min(effect.remainingHits, 10);

            // Obra¿enia = Baza (np. 10% hita) * efektywne stacki (max 10)
            int damage = effect.value * effectiveStacks;

            // Wyœwietlamy napis i zadajemy obra¿enia
            owner.ShowFloatingText(effect.effectName + "!", DamagePopup.PopupType.Miss);
            owner.TakeDamage(damage, false, "", true, SkillCategory.MeleePhysical);

            // Zaka¿enie s³abnie! Odejmujemy jeden stack.
            effect.remainingHits--;

            // Aktualizujemy UI, ¿eby cyferka spada³a z ka¿d¹ rund¹
            effect.duration = effect.remainingHits;
        }
    }
    public override bool IsExpired(StatusEffect effect)
    {
        return effect.remainingHits <= 0;
    }
}