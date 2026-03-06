using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillAPHandler : MonoBehaviour
{
    public List<Image> apSquares; // Tu przeci¹gnij 6 obrazków kwadracików
    public Sprite activeSprite;   // Twoja grafika "naklikanego" kafelka
    public Sprite inactiveSprite; // Twoja grafika pustego kafelka

    private int investedAP = 0;

    // Tê funkcjê bêdziemy wywo³ywaæ klikniêciem w kwadracik
    public void OnSquareClicked(int index)
    {
        int targetLevel = index + 1;

        // NOWOŒÆ: Jeœli klikamy dok³adnie w ten sam poziom, który ju¿ mamy zaznaczony
        if (targetLevel == investedAP)
        {
            // Cofamy o 1 punkt (czyli np. z 1 na 0)
            CombatAPManager.Instance.RefundAP(1);
            investedAP--;
        }
        else if (targetLevel > investedAP)
        {
            int cost = targetLevel - investedAP;
            if (CombatAPManager.Instance.TrySpendAP(cost))
            {
                investedAP = targetLevel;
            }
        }
        else if (targetLevel < investedAP) // Zmienione z <= na <
        {
            int refund = investedAP - targetLevel;
            CombatAPManager.Instance.RefundAP(refund);
            investedAP = targetLevel;
        }

        UpdateVisuals();
    }

    // Opcjonalnie: prawy klik na ikonê skilla czyœci wszystko
    public void ClearAll()
    {
        CombatAPManager.Instance.RefundAP(investedAP);
        investedAP = 0;
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        for (int i = 0; i < apSquares.Count; i++)
        {
            apSquares[i].sprite = (i < investedAP) ? activeSprite : inactiveSprite;
        }
    }

    [Header("Dane Umiejêtnoœci")]
    public SkillData currentSkill; // Tu zapiszemy wybrany skill
    public Image mainIconDisplay; // Przeci¹gnij tu obrazek "Icon" ze swojego kó³ka

    [Header("Baza Skilli (Do testów)")]
    // TU PRZECI¥GNIESZ SWOJE 6 PLIKÓW SKILLI
    public List<SkillData> testAvailableSkills;

    // TEJ FUNKCJI BRAKOWA£O (Naprawia b³¹d w konsoli)
    public void AssignSkill(SkillData data)
    {
        currentSkill = data;
        if (mainIconDisplay != null)
        {
            mainIconDisplay.sprite = data.icon;
            mainIconDisplay.color = Color.white; // Upewnij siê, ¿e ikona jest widoczna
        }
        Debug.Log("Przypisano skill: " + data.skillName);
    }

    // Tê funkcjê podepniemy pod Button g³ównej ikony w kó³ku
    public void OpenSkillSelection()
    {
        SkillSelectionWindow.Instance.Open(this, testAvailableSkills);
    }
}