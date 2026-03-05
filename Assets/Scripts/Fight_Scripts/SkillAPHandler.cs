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
}