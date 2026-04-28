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
        ShowTooltip(item, null);
    }

    public void ShowTooltip(EquipmentItemData item, DraggableItem sourceItem)
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

        // Per-instance upgrade bonuses
        if (sourceItem != null && sourceItem.upgradePoints != null)
        {
            int[] p = new int[DraggableItem.UPGRADE_STAT_COUNT];
            int n = Mathf.Min(p.Length, sourceItem.upgradePoints.Count);
            for (int i = 0; i < n; i++) p[i] = sourceItem.upgradePoints[i];

            if (p[1] > 0) s += $"+{p[1] * 10} PZ <color=#7CFC00>(ulepszenia)</color>\n";
            if (p[2] > 0) s += $"+{p[2]} Siły <color=#7CFC00>(ulepszenia)</color>\n";
            if (p[3] > 0) s += $"+{p[3]} Zręczności <color=#7CFC00>(ulepszenia)</color>\n";
            if (p[4] > 0) s += $"+{p[4] * 10} Kondycji <color=#7CFC00>(ulepszenia)</color>\n";
            if (p[5] > 0) s += $"+{p[5] * 10} Many <color=#7CFC00>(ulepszenia)</color>\n";
            if (p[6] > 0) s += $"+{p[6]} Pancerz Fizyczny <color=#7CFC00>(ulepszenia)</color>\n";
            if (p[7] > 0) s += $"+{p[7]} Pancerz Magiczny <color=#7CFC00>(ulepszenia)</color>\n";
            if (p[8] > 0) s += $"+{p[8]} Obrażeń Broni <color=#7CFC00>(ulepszenia)</color>\n";
            if (p[9] > 0) s += $"+{p[9]}% Szansa na Kryt <color=#7CFC00>(ulepszenia)</color>\n";
            if (p[10] > 0) s += $"+{p[10]}% Szansa na Unik <color=#7CFC00>(ulepszenia)</color>\n";
            if (p[11] > 0) s += $"+{p[11] * 5}% Mnożnik Obrażeń <color=#7CFC00>(ulepszenia)</color>\n";
            if (p[12] > 0) s += $"+{p[12] * 5}% Mnożnik Trafienia <color=#7CFC00>(ulepszenia)</color>\n";
        }
        
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