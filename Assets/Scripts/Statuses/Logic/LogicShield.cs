using UnityEngine;

public class LogicShield : StatusLogic
{
    public override int OnTakeDamage(Combatant owner, StatusEffect effect, int incomingDamage, bool isDot, SkillCategory category)
    {
        // Tarcza ignoruje truciznê i ataki psychiczne
        if (isDot || category == SkillCategory.NegativeCharm)
        {
            return incomingDamage;
        }

        if (effect.remainingHits > 0 && incomingDamage > 0)
        {
            effect.remainingHits--; // Zu¿ywamy ³adunek bloku

            // --- OBLICZANIE REDUKCJI PROCENTOWEJ ---
            float blockPercent = Mathf.Clamp(effect.value, 0f, 100f); // Max 100%
            float damageMultiplier = 1f - (blockPercent / 100f);

            int reducedDamage = Mathf.RoundToInt(incomingDamage * damageMultiplier);

            // --- NOWOŒÆ: Odtwarzamy efekt wizualny (VFX) z ikonk¹ zamiast tekstu! ---
            if (effect.icon != null)
            {
                owner.PlaySkillEffect(effect.icon);
            }

            // Zwracamy ZMNIEJSZONE obra¿enia dalej do systemu
            return reducedDamage;
        }
        return incomingDamage;
    }

    public override bool IsExpired(StatusEffect effect)
    {
        return effect.remainingHits <= 0 || effect.duration <= 0;
    }
}