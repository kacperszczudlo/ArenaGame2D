using UnityEngine;
using TMPro;

public class ItemTooltip : MonoBehaviour
{
    public static ItemTooltip Instance;
    
    [Header("Podepnij teksty z Tooltip_Panel")]
    public TMP_Text nameText;
    public TMP_Text statsText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        gameObject.SetActive(false); 
    }

    private void Update()
    {
        // Okienko podąża za kursorem
        transform.position = Input.mousePosition + new Vector3(15, -15, 0);
    }

    public void ShowTooltip(EquipmentItemData item)
    {
        // SZPIEG 2: Sprawdzamy czy Tooltip dostał rozkaz pokazania się
        Debug.Log($"[TOOLTIP] Próbuję pokazać statystyki dla: {item.itemName}");

        gameObject.SetActive(true);
        transform.SetAsLastSibling(); 

        nameText.text = item.itemName;

        string s = "";
        if (item.bonusPhysicalArmor > 0) s += $"+{item.bonusPhysicalArmor} Pancerz Fizyczny\n";
        if (item.bonusMagicResistance > 0) s += $"+{item.bonusMagicResistance} Pancerz Magiczny\n";
        if (item.bonusMaxHP > 0) s += $"+{item.bonusMaxHP} PZ\n";
        if (item.bonusMaxMana > 0) s += $"+{item.bonusMaxMana} Many\n";
        if (item.bonusMaxStamina > 0) s += $"+{item.bonusMaxStamina} Kondycji\n";
        if (item.bonusStrength > 0) s += $"+{item.bonusStrength} Siły\n";
        if (item.bonusAgility > 0) s += $"+{item.bonusAgility} Zręczności\n";
        if (item.bonusKnowledge > 0) s += $"+{item.bonusKnowledge} Wiedzy\n";
        if (item.bonusPower > 0) s += $"+{item.bonusPower} Mocy\n";
        if (item.weaponDamage > 0) s += $"+{item.weaponDamage} Obrażeń Broni\n";
        if (item.bonusCritChance > 0) s += $"+{item.bonusCritChance}% Szansa na Kryt\n";
        if (item.bonusDodgeChance > 0) s += $"+{item.bonusDodgeChance}% Szansa na Unik\n";
        
        s += $"\n<color=yellow>Wartość: {item.sellPrice}g</color>";
        statsText.text = s;
    }

    public void HideTooltip()
    {
        if (gameObject.activeSelf)
        {
            Debug.Log("[TOOLTIP] Chowam okienko");
            gameObject.SetActive(false);
        }
    }
}