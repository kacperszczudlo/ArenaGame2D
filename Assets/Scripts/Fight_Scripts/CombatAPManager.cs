using UnityEngine;
using TMPro;

public class CombatAPManager : MonoBehaviour
{
    public static CombatAPManager Instance;

    public int maxAP = 15;
    public int currentAP;

    public TextMeshProUGUI currentText;
    public TextMeshProUGUI maxText;

    void Awake() { Instance = this; }

    void Start()
    {
        currentAP = maxAP;
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
        currentAP = Mathf.Min(currentAP + amount, maxAP);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (currentText) currentText.text = currentAP.ToString();
        if (maxText) maxText.text = maxAP.ToString();
    }
}