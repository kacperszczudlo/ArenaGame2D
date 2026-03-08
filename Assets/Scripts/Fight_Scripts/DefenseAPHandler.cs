using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// Enum okreœlaj¹cy, czym jest to konkretne kó³ko
public enum DefenseCategory { Melee, Ranged, Mental }

public class DefenseAPHandler : MonoBehaviour
{
    [Header("Typ Obrony (Ustaw w Inspektorze!)")]
    public DefenseCategory defenseCategory;

    [Header("Punkty Akcji (PA)")]
    public List<Image> apSquares; // Kwadraciki
    public Sprite activeSprite;   // Pe³ny kwadracik
    public Sprite inactiveSprite; // Pusty kwadracik
    public int currentPA = 0;

    // Zauwa¿ brak AssignSkill i OpenSkillSelection!

    public void OnSquareClicked(int index)
    {
        int targetLevel = index + 1;

        if (targetLevel == currentPA)
        {
            if (CombatAPManager.Instance != null) CombatAPManager.Instance.RefundAP(1);
            currentPA--;
        }
        else if (targetLevel > currentPA)
        {
            int cost = targetLevel - currentPA;
            if (CombatAPManager.Instance == null || CombatAPManager.Instance.TrySpendAP(cost))
            {
                currentPA = targetLevel;
            }
        }
        else if (targetLevel < currentPA)
        {
            int refund = currentPA - targetLevel;
            if (CombatAPManager.Instance != null) CombatAPManager.Instance.RefundAP(refund);
            currentPA = targetLevel;
        }

        UpdateVisuals();
    }

    public void ClearAll()
    {
        if (CombatAPManager.Instance != null) CombatAPManager.Instance.RefundAP(currentPA);
        currentPA = 0;
        UpdateVisuals();
    }

    public void UpdateVisuals() // Zmienione na publiczne, ¿eby BattleManager móg³ to odœwie¿aæ
    {
        for (int i = 0; i < apSquares.Count; i++)
        {
            apSquares[i].sprite = (i < currentPA) ? activeSprite : inactiveSprite;
        }
    }
}