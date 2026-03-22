using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillAPHandler : MonoBehaviour
{
    [Header("Punkty Akcji (PA)")]
    public List<Image> apSquares; // obrazki kwadracików
    public Sprite activeSprite;   // Grafika "naklikanego" kafelka
    public Sprite inactiveSprite; // Grafika pustego kafelka
    public int currentPA = 0;

    [Header("Dane Umiejêtnoœci")]
    public CharacterSkill currentSkill; 
    public Image mainIconDisplay;

    // JEDNA, POPRAWNA funkcja do przypisywania skilli
    public void AssignSkill(CharacterSkill cSkill)
    {
        currentSkill = cSkill;
        if (mainIconDisplay != null && cSkill.data != null)
        {
            mainIconDisplay.sprite = cSkill.data.icon;
            mainIconDisplay.color = Color.white;
            Debug.Log("Przypisano skill: " + cSkill.data.skillName);
        }
    }

    public void OpenSkillSelection()
    {
        // Kó³ko szuka na scenie BattleManagera, bierze od niego Gracza (Rycerza) i jego osobiste skille
        BattleManager manager = Object.FindFirstObjectByType<BattleManager>();
        if (manager != null && manager.player != null)
        {
            SkillSelectionWindow.Instance.Open(this, manager.player.mySkills);
        }
        else
        {
            Debug.LogWarning("Nie znaleziono BattleManagera lub Gracza na scenie!");
        }
    }

    // Funkcja wywo³ywana klikniêciem w kwadracik
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

    public void UpdateVisuals()
    {
        for (int i = 0; i < apSquares.Count; i++)
        {
            apSquares[i].sprite = (i < currentPA) ? activeSprite : inactiveSprite;
        }
    }
}