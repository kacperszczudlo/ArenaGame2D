using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsWindow : MonoBehaviour
{
    [Header("Lewy Panel - G³ówne Informacje")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI gemsText;
    public TextMeshProUGUI deathCountText;

    [Header("Lewy Panel - Pasek Expa")]
    public Image expFillBar;
    public TextMeshProUGUI expCurrentText;
    public TextMeshProUGUI expMaxText;

    [Header("Prawy Panel - Punkty")]
    public TextMeshProUGUI availablePointsText;

    [Header("Statystyki (Baza / Ca³kowite)")]
    public TextMeshProUGUI strBaseText; public TextMeshProUGUI strTotalText; public Button strBtn;
    public TextMeshProUGUI agiBaseText; public TextMeshProUGUI agiTotalText; public Button agiBtn;
    public TextMeshProUGUI powBaseText; public TextMeshProUGUI powTotalText; public Button powBtn;
    public TextMeshProUGUI knoBaseText; public TextMeshProUGUI knoTotalText; public Button knoBtn;
    public TextMeshProUGUI hpBaseText; public TextMeshProUGUI hpTotalText; public Button hpBtn;
    public TextMeshProUGUI manaBaseText; public TextMeshProUGUI manaTotalText; public Button manaBtn;
    public TextMeshProUGUI stamBaseText; public TextMeshProUGUI stamTotalText; public Button stamBtn;

    [Header("Pancerze i Odpornoœci (Tylko Odczyt z EQ)")]
    public TextMeshProUGUI physicalArmorTotalText;
    public TextMeshProUGUI magicArmorTotalText;

    void Start()
    {
        RefreshWindow();
    }

    void OnEnable()
    {
        RefreshWindow();
    }

    public void RefreshWindow()
    {
        if (PlayerDataManager.Instance == null || GameManager.Instance == null) return;

        var data = PlayerDataManager.Instance;

        if (levelText != null) levelText.text = $"Obecny poziom postaci: {data.currentLevel}";
        if (goldText != null) goldText.text = $"Z³oto: {GameManager.Instance.globalGold}";
        if (gemsText != null) gemsText.text = $"Gemy turniejowe: {GameManager.Instance.tournamentGems}";
        if (deathCountText != null) deathCountText.text = $"Liczba zgonów: {data.deathCount}";

        if (data.currentLevel >= 35)
        {
            if (expCurrentText != null) expCurrentText.text = "MAX";
            if (expMaxText != null) expMaxText.text = "MAX";
            if (expFillBar != null) expFillBar.fillAmount = 1f;
        }
        else
        {
            int reqExp = data.GetRequiredExpForNextLevel();
            if (expCurrentText != null) expCurrentText.text = data.currentExperience.ToString();
            if (expMaxText != null) expMaxText.text = reqExp.ToString();
            if (expFillBar != null) expFillBar.fillAmount = (float)data.currentExperience / reqExp;
        }

        if (availablePointsText != null) availablePointsText.text = $"Dostêpne punkty: {data.availableStatPoints}";

        bool canUpgrade = data.availableStatPoints > 0;
        if (strBtn != null) strBtn.interactable = canUpgrade;
        if (agiBtn != null) agiBtn.interactable = canUpgrade;
        if (powBtn != null) powBtn.interactable = canUpgrade;
        if (knoBtn != null) knoBtn.interactable = canUpgrade;
        if (hpBtn != null) hpBtn.interactable = canUpgrade;
        if (manaBtn != null) manaBtn.interactable = canUpgrade;
        if (stamBtn != null) stamBtn.interactable = canUpgrade;

        // --- ZAMIAST ZER, CZYTAMY TWOJE BONUSY Z MENED¯ERA! ---
        int eqStr = data.bonusStrength;
        int eqAgi = data.bonusAgility;
        int eqPow = data.bonusPower;
        int eqKno = data.bonusKnowledge;

        int eqHp = data.bonusMaxHP;
        int eqMana = data.bonusMaxMana;
        int eqStam = data.bonusMaxStamina;

        int eqArmorPhys = data.bonusPhysicalArmor;
        int eqArmorMag = data.bonusMagicResistance;

        // Podpinamy do UI
        UpdateStatUI(data.baseStrength, eqStr, strBaseText, strTotalText);
        UpdateStatUI(data.baseAgility, eqAgi, agiBaseText, agiTotalText);
        UpdateStatUI(data.basePower, eqPow, powBaseText, powTotalText);
        UpdateStatUI(data.baseKnowledge, eqKno, knoBaseText, knoTotalText);

        UpdateStatUI(data.baseMaxHP, eqHp, hpBaseText, hpTotalText);
        UpdateStatUI(data.baseMaxMana, eqMana, manaBaseText, manaTotalText);
        UpdateStatUI(data.baseMaxStamina, eqStam, stamBaseText, stamTotalText);

        // Pancerze czytaj¹ bazê gracza + bonus z ekwipunku
        UpdateStatUI(data.basePhysicalArmor, eqArmorPhys, null, physicalArmorTotalText);
        UpdateStatUI(data.baseMagicResistance, eqArmorMag, null, magicArmorTotalText);
    }

    private void UpdateStatUI(int baseValue, int eqBonus, TextMeshProUGUI baseText, TextMeshProUGUI totalText)
    {
        if (baseText != null) baseText.text = baseValue.ToString();

        if (totalText != null)
        {
            int total = baseValue + eqBonus;
            totalText.text = total.ToString();

            totalText.color = Color.green;
        }
    }

    // --- FUNKCJE PODPINANE POD PRZYCISKI "+" ---
    public void UpgradeStrength()
    {
        if (SpendPoint())
        {
            PlayerDataManager.Instance.baseStrength++;
            PlayerDataManager.Instance.SavePlayerData(); // Zapis!
        }
        RefreshWindow();
    }

    public void UpgradeAgility()
    {
        if (SpendPoint())
        {
            PlayerDataManager.Instance.baseAgility++;
            PlayerDataManager.Instance.SavePlayerData(); // Zapis!
        }
        RefreshWindow();
    }

    public void UpgradePower()
    {
        if (SpendPoint())
        {
            PlayerDataManager.Instance.basePower++;
            PlayerDataManager.Instance.SavePlayerData(); // Zapis!
        }
        RefreshWindow();
    }

    public void UpgradeKnowledge()
    {
        if (SpendPoint())
        {
            PlayerDataManager.Instance.baseKnowledge++;
            PlayerDataManager.Instance.SavePlayerData(); // Zapis!
        }
        RefreshWindow();
    }

    // Przeliczniki x10!
    public void UpgradeHP()
    {
        if (SpendPoint())
        {
            PlayerDataManager.Instance.baseMaxHP += 10;
            PlayerDataManager.Instance.SavePlayerData(); // Zapis!
        }
        RefreshWindow();
    }

    public void UpgradeMana()
    {
        if (SpendPoint())
        {
            PlayerDataManager.Instance.baseMaxMana += 10;
            PlayerDataManager.Instance.SavePlayerData(); // Zapis!
        }
        RefreshWindow();
    }

    public void UpgradeStamina()
    {
        if (SpendPoint())
        {
            PlayerDataManager.Instance.baseMaxStamina += 10;
            PlayerDataManager.Instance.SavePlayerData(); // Zapis!
        }
        RefreshWindow();
    }

    private bool SpendPoint()
    {
        if (PlayerDataManager.Instance.availableStatPoints > 0)
        {
            PlayerDataManager.Instance.availableStatPoints--;
            return true;
        }
        return false;
    }

    public void CloseWindow()
    {
        gameObject.SetActive(false);
    }
}