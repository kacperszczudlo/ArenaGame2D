using UnityEngine;

public class LogicFreeze : StatusLogic
{
    public override void OnTurnStart(Combatant owner, StatusEffect effect)
    {
        int staminaDrain = Mathf.RoundToInt(effect.multiplier);
        if (staminaDrain <= 0) staminaDrain = 15;

        owner.currentStamina -= staminaDrain;
        if (owner.currentStamina < 0) owner.currentStamina = 0;

        // --- NOWOŒÆ: ODŒWIE¯AMY UI NATYCHMIAST! ---
        if (owner.myUI != null) owner.myUI.UpdateUI();

        owner.ShowFloatingText($"-{staminaDrain} Kondycji", DamagePopup.PopupType.Miss);
    }

    public override bool IsExpired(StatusEffect effect)
    {
        return effect.duration <= 0;
    }
}