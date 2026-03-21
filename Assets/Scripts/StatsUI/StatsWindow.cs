using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsWindow : MonoBehaviour
{
    [Header("G³ówne Informacje")]
    public TextMeshProUGUI availablePointsText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI expText;
    public Image expFillBar; // Pasek doœwiadczenia
    public TextMeshProUGUI deathCountText;

    [Header("Waluty")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI gemsText;

    [Header("Si³a")]
    public TextMeshProUGUI strBaseText;
    public TextMeshProUGUI strTotalText;
    public Button strBtn;

    [Header("Zrêcznoœæ")]
    public TextMeshProUGUI agiBaseText;
    public TextMeshProUGUI agiTotalText;
    public Button agiBtn;

    [Header("Moc")]
    public TextMeshProUGUI powBaseText;
    public TextMeshProUGUI powTotalText;
    public Button powBtn;

    [Header("Wiedza")]
    public TextMeshProUGUI knoBaseText;
    public TextMeshProUGUI knoTotalText;
    public Button knoBtn;

    [Header("Punkty ¯ycia (HP)")]
    public TextMeshProUGUI hpBaseText;
    public TextMeshProUGUI hpTotalText;
    public Button hpBtn;

    [Header("Mana")]
    public TextMeshProUGUI manaBaseText;
    public TextMeshProUGUI manaTotalText;
    public Button manaBtn;

    [Header("Kondycja")]
    public TextMeshProUGUI stamBaseText;
    public TextMeshProUGUI stamTotalText;
    public Button stamBtn;

    void OnEnable()
    {
        RefreshWindow();
    }

    public void RefreshWindow()
    {
        if (PlayerDataManager.Instance == null || GameManager.Instance == null) return;

        var data = PlayerDataManager.Instance;

        // 1. G³ówne informacje i Pasek Expa
        availablePointsText.text = data.availableStatPoints.ToString();
        levelText.text = $"Poziom: {data.currentLevel}";

        int reqExp = data.GetRequiredExpForNextLevel();
        expText.text = $"{data.currentExperience} / {reqExp}";
        if (expFillBar != null) expFillBar.fillAmount = (float)data.currentExperience / reqExp;

        deathCountText.text = $"Poleg³ w walce: {data.deathCount} razy";

        goldText.text = GameManager.Instance.globalGold.ToString();
        gemsText.text = GameManager.Instance.tournamentGems.ToString();

        // 2. Czy mo¿na rozdawaæ punkty?
        bool canUpgrade = data.availableStatPoints > 0;
        strBtn.interactable = canUpgrade;
        agiBtn.interactable = canUpgrade;
        powBtn.interactable = canUpgrade;
        knoBtn.interactable = canUpgrade;
        hpBtn.interactable = canUpgrade;
        manaBtn.interactable = canUpgrade;
        stamBtn.interactable = canUpgrade;

        // 3. Wypisywanie statystyk (Bia³e = Baza, Zielone = Baza + Ekwipunek)
        // TODO: W przysz³oœci dodamy tu pobieranie bonusów z ekwipunku!
        int eqStr = 0, eqAgi = 0, eqPow = 0, eqKno = 0, eqHp = 0, eqMana = 0, eqStam = 0;

        UpdateStatUI(data.baseStrength, eqStr, strBaseText, strTotalText);
        UpdateStatUI(data.baseAgility, eqAgi, agiBaseText, agiTotalText);
        UpdateStatUI(data.basePower, eqPow, powBaseText, powTotalText);
        UpdateStatUI(data.baseKnowledge, eqKno, knoBaseText, knoTotalText);
        UpdateStatUI(data.baseMaxHP, eqHp, hpBaseText, hpTotalText);
        UpdateStatUI(data.baseMaxMana, eqMana, manaBaseText, manaTotalText);
        UpdateStatUI(data.baseMaxStamina, eqStam, stamBaseText, stamTotalText);
    }

    private void UpdateStatUI(int baseValue, int eqBonus, TextMeshProUGUI baseText, TextMeshProUGUI totalText)
    {
        if (baseText != null) baseText.text = baseValue.ToString();
        if (totalText != null)
        {
            int total = baseValue + eqBonus;
            totalText.text = total.ToString();
            totalText.color = eqBonus > 0 ? Color.green : new Color(0.2f, 0.8f, 0.2f); // Jaœniejszy zielony jeœli jest bonus
        }
    }

    // --- FUNKCJE PODPINANE POD PRZYCISKI "+" ---

    public void UpgradeStrength() { if (SpendPoint()) PlayerDataManager.Instance.baseStrength++; RefreshWindow(); }
    public void UpgradeAgility() { if (SpendPoint()) PlayerDataManager.Instance.baseAgility++; RefreshWindow(); }
    public void UpgradePower() { if (SpendPoint()) PlayerDataManager.Instance.basePower++; RefreshWindow(); }
    public void UpgradeKnowledge() { if (SpendPoint()) PlayerDataManager.Instance.baseKnowledge++; RefreshWindow(); }

    // Przeliczniki x10!
    public void UpgradeHP() { if (SpendPoint()) PlayerDataManager.Instance.baseMaxHP += 10; RefreshWindow(); }
    public void UpgradeMana() { if (SpendPoint()) PlayerDataManager.Instance.baseMaxMana += 10; RefreshWindow(); }
    public void UpgradeStamina() { if (SpendPoint()) PlayerDataManager.Instance.baseMaxStamina += 10; RefreshWindow(); }

    private bool SpendPoint()
    {
        if (PlayerDataManager.Instance.availableStatPoints > 0)
        {
            PlayerDataManager.Instance.availableStatPoints--;
            return true;
        }
        return false;
    }
}