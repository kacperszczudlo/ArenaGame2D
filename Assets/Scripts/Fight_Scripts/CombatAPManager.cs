using UnityEngine;
using TMPro;

public class CombatAPManager : MonoBehaviour
{
    public static CombatAPManager Instance;

    public int maxAP = 15; // Absolutny, sta³y limit postaci
    public int currentAP;

    [HideInInspector]
    public int effectiveMaxAP; // "Ruchomy sufit", który obni¿aj¹ kl¹twy (Mrozu/Œlepoty)

    public TextMeshProUGUI currentText;
    public TextMeshProUGUI maxText;

    void Awake() { Instance = this; }

    void Start()
    {
        effectiveMaxAP = maxAP; // Na pocz¹tku walki sufit jest w pe³ni si³
        currentAP = effectiveMaxAP;
        UpdateUI();
    }

    public bool TrySpendAP(int amount)
    {
        if (currentAP >= amount)
        {
            currentAP -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    public void RefundAP(int amount)
    {
        // ZMIANA: Kiedy odklikujesz kó³ko, punkty mog¹ wróciæ tylko do obecnego, RUCHOMEGO limitu!
        currentAP = Mathf.Min(currentAP + amount, effectiveMaxAP);
        UpdateUI();
    }

    // Tê funkcjê wywo³uje BattleManager za ka¿dym razem, gdy przelicza kary z debuffów
    public void UpdateAPText(int newEffectiveMax)
    {
        effectiveMaxAP = newEffectiveMax; // Zapisujemy nowy, okrojony sufit
        UpdateUI(); // Odœwie¿amy ekrany
    }

    void UpdateUI()
    {
        if (currentText) currentText.text = currentAP.ToString();

        // ZMIANA: Pokazujemy w UI "Ruchomy Sufit", a nie sta³e 15!
        if (maxText) maxText.text = effectiveMaxAP.ToString();
    }
}