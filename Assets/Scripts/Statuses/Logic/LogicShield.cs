using UnityEngine;

public class LogicShield : StatusLogic
{
    public override int OnTakeDamage(Combatant owner, StatusEffect effect, int incomingDamage)
    {
        // Jeœli mamy ³adunki tarczy i w ogóle s¹ jakieœ obra¿enia do zablokowania
        if (effect.remainingHits > 0 && incomingDamage > 0)
        {
            float reduction = effect.value / 100f;
            int finalDamage = Mathf.RoundToInt(incomingDamage * (1f - reduction));

            effect.remainingHits--;

            // Mo¿emy tu zostawiæ wyœwietlanie napisu o bloku!
            owner.ShowFloatingText("Blok!", DamagePopup.PopupType.NormalDamage);

            return finalDamage;
        }
        return incomingDamage;
    }

    public override bool IsExpired(StatusEffect effect)
    {
        return effect.duration <= 0 || effect.remainingHits <= 0;
    }
}