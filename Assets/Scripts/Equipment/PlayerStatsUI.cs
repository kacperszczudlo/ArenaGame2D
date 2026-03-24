using UnityEngine;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    public static PlayerStatsUI Instance;

    [Header("BIAŁE TEKSTY (Zmienne Bazowe)")]
    public TMP_Text hpBase; public TMP_Text manaBase; public TMP_Text staminaBase;
    public TMP_Text strBase; public TMP_Text agiBase; public TMP_Text powerBase; public TMP_Text knowBase;
    public TMP_Text armorFizBase; public TMP_Text armorMagBase;

    [Header("ZIELONE TEKSTY (Bonus z Ekwipunku)")]
    public TMP_Text hpBonus; public TMP_Text manaBonus; public TMP_Text staminaBonus;
    public TMP_Text strBonus; public TMP_Text agiBonus; public TMP_Text powerBonus; public TMP_Text knowBonus;
    public TMP_Text armorFizBonus; public TMP_Text armorMagBonus;

    private void Awake()
    {
        Instance = this;
    }

    // Funkcja wbudowana w Unity - wywołuje się automatycznie, gdy otworzysz ekwipunek
    private void OnEnable()
    {
        UpdateStatsUI();
    }

    public void UpdateStatsUI()
    {
        if (PlayerDataManager.Instance == null) return;
        
        // Zapisujemy menedżera do krótkiej zmiennej 'p', by nie pisać tego w kółko
        PlayerDataManager p = PlayerDataManager.Instance;

        // 1. Wypisujemy teksty bazowe
        if(hpBase) hpBase.text = p.baseMaxHP.ToString();
        if(manaBase) manaBase.text = p.baseMaxMana.ToString();
        if(staminaBase) staminaBase.text = p.baseMaxStamina.ToString();
        if(strBase) strBase.text = p.baseStrength.ToString();
        if(agiBase) agiBase.text = p.baseAgility.ToString();
        if(powerBase) powerBase.text = p.basePower.ToString();
        if(knowBase) knowBase.text = p.baseKnowledge.ToString();
        if(armorFizBase) armorFizBase.text = p.basePhysicalArmor.ToString();
        if(armorMagBase) armorMagBase.text = p.baseMagicResistance.ToString();

        // 2. Wypisujemy teksty z bonusami
        SetBonusText(hpBonus, p.bonusMaxHP);
        SetBonusText(manaBonus, p.bonusMaxMana);
        SetBonusText(staminaBonus, p.bonusMaxStamina);
        SetBonusText(strBonus, p.bonusStrength);
        SetBonusText(agiBonus, p.bonusAgility);
        SetBonusText(powerBonus, p.bonusPower);
        SetBonusText(knowBonus, p.bonusKnowledge);
        SetBonusText(armorFizBonus, p.bonusPhysicalArmor);
        SetBonusText(armorMagBonus, p.bonusMagicResistance);
    }

    // Pomocnicza funkcja formatująca
    private void SetBonusText(TMP_Text txt, int value)
    {
        if (txt == null) return;

        if (value > 0)
        {
            txt.text = $"+{value}";
            txt.color = Color.green;
        }
        else if (value < 0)
        {
            txt.text = $"{value}";
            txt.color = Color.red; // Gdyby postać założyła przeklęty przedmiot
        }
        else
        {
            txt.text = "+0"; // Jeśli przedmiot nie dodaje tej statystyki
            txt.color = new Color(0.2f, 0.8f, 0.2f); // Ciemniejszy zielony by nie rzucał się w oczy
        }
    }
}