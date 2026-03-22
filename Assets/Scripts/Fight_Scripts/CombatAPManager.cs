using UnityEngine;
using TMPro;

public class CombatAPManager : MonoBehaviour
{
    public static CombatAPManager Instance;

    public int maxAP = 15;
    public int currentAP;

    [HideInInspector]
    public int effectiveMaxAP; // ruchome pa który obni¿aj¹ kl¹twy

    public TextMeshProUGUI currentText;
    public TextMeshProUGUI maxText;

    void Awake() { Instance = this; }

    void Start()
    {
        effectiveMaxAP = maxAP;
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
        currentAP = Mathf.Min(currentAP + amount, effectiveMaxAP);
        UpdateUI();
    }

    // Tê funkcjê wywo³uje BattleManager za ka¿dym razem, gdy przelicza kary z debuffów
    public void UpdateAPText(int newEffectiveMax)
    {
        effectiveMaxAP = newEffectiveMax; 
        UpdateUI();
    }

    void UpdateUI()
    {
        if (currentText) currentText.text = currentAP.ToString();

        if (maxText) maxText.text = effectiveMaxAP.ToString();
    }
}