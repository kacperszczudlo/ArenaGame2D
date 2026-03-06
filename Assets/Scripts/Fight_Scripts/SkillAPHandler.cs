using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillAPHandler : MonoBehaviour
{
    [Header("Punkty Akcji (PA)")]
    public List<Image> apSquares; // Tu przeci¹gnij obrazki kwadracików
    public Sprite activeSprite;   // Grafika "naklikanego" kafelka
    public Sprite inactiveSprite; // Grafika pustego kafelka

    // ZMIANA: Zamieniliœmy private investedAP na public currentPA, 
    // ¿eby BattleManager wiedzia³, ile punktów wydano na ten atak!
    public int currentPA = 0;

    [Header("Dane Umiejêtnoœci")]
    public SkillData currentSkill;
    public Image mainIconDisplay;

    [Header("Baza Skilli (Do testów)")]
    public List<SkillData> testAvailableSkills;

    // Funkcja wywo³ywana klikniêciem w kwadracik
    public void OnSquareClicked(int index)
    {
        int targetLevel = index + 1;

        // Jeœli klikamy dok³adnie w ten sam poziom, który ju¿ mamy zaznaczony
        if (targetLevel == currentPA)
        {
            // Cofamy o 1 punkt (czyli np. z 1 na 0)
            if (CombatAPManager.Instance != null) CombatAPManager.Instance.RefundAP(1);
            currentPA--;
        }
        else if (targetLevel > currentPA)
        {
            int cost = targetLevel - currentPA;
            // Zabezpieczenie na wypadek braku mened¿era na scenie
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

    // Opcjonalnie: prawy klik na ikonê skilla czyœci wszystko
    public void ClearAll()
    {
        if (CombatAPManager.Instance != null) CombatAPManager.Instance.RefundAP(currentPA);
        currentPA = 0;
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        for (int i = 0; i < apSquares.Count; i++)
        {
            apSquares[i].sprite = (i < currentPA) ? activeSprite : inactiveSprite;
        }
    }

    public void AssignSkill(SkillData data)
    {
        currentSkill = data;
        if (mainIconDisplay != null)
        {
            mainIconDisplay.sprite = data.icon;
            mainIconDisplay.color = Color.white;
        }
        Debug.Log("Przypisano skill: " + data.skillName);
    }

    public void OpenSkillSelection()
    {
        SkillSelectionWindow.Instance.Open(this, testAvailableSkills);
    }
}