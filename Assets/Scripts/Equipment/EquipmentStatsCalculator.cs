using UnityEngine;

public class EquipmentStatsCalculator : MonoBehaviour
{
    public static EquipmentStatsCalculator Instance;

    [Header("Podepnij rodzica slotów postaci (LeftColumn_Equip/Equipment_Area)")]
    public Transform equippedSlotsContainer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    // Ta funkcja będzie wywoływana za każdym razem, gdy upuścisz przedmiot na jakiś slot
    public void RecalculateAllEquipmentStats()
    {
        if (PlayerDataManager.Instance == null)
        {
            Debug.LogError("Brak PlayerDataManager na scenie!");
            return;
        }

        if (equippedSlotsContainer == null) return;

        // 1. Zerujemy wszystkie bonusy w menedżerze przed podliczeniem
        PlayerDataManager p = PlayerDataManager.Instance;
        p.bonusMaxHP = 0; p.bonusMaxMana = 0; p.bonusMaxStamina = 0;
        p.bonusStrength = 0; p.bonusAgility = 0; p.bonusKnowledge = 0; p.bonusPower = 0;
        p.bonusPhysicalArmor = 0; p.bonusMagicResistance = 0;
        p.weaponDamage = 0;
        p.bonusCritChance = 0; p.bonusDodgeChance = 0;
        p.bonusDamageMultiplier = 0f; p.bonusHitChanceMultiplier = 0f;

        // 2. Szukamy przedmiotów założonych NA CIAŁO (tylko w tych okienkach po lewej)
        foreach (Transform slot in equippedSlotsContainer)
        {
            ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
            
            // Sprawdzamy czy to okienko to "ciało" i czy w ogóle ma w sobie przedmiot
            if (itemSlot != null && itemSlot.isEquippedSlot && slot.childCount > 0)
            {
                DraggableItem item = slot.GetChild(0).GetComponent<DraggableItem>();
                if (item != null && item.itemData != null)
                {
                    EquipmentItemData d = item.itemData;
                    
                    // 3. Zsumowanie statystyk i dodanie do PlayerDataManagera
                    p.bonusMaxHP += d.bonusMaxHP;
                    p.bonusMaxMana += d.bonusMaxMana;
                    p.bonusMaxStamina += d.bonusMaxStamina;
                    
                    p.bonusStrength += d.bonusStrength;
                    p.bonusAgility += d.bonusAgility;
                    p.bonusKnowledge += d.bonusKnowledge;
                    p.bonusPower += d.bonusPower;
                    
                    p.bonusPhysicalArmor += d.bonusPhysicalArmor;
                    p.bonusMagicResistance += d.bonusMagicResistance;
                    
                    if (d.itemType == ItemType.Weapon) p.weaponDamage += d.weaponDamage;

                    p.bonusCritChance += d.bonusCritChance;
                    p.bonusDodgeChance += d.bonusDodgeChance;
                    p.bonusDamageMultiplier += d.bonusDamageMultiplier;
                    p.bonusHitChanceMultiplier += d.bonusHitChanceMultiplier;
                }
            }
        }
        
        Debug.Log($"[KALKULATOR] Podliczono nowy ekwipunek! Całkowita dodatkowa siła: {p.bonusStrength}, Pancerz fiz.: {p.bonusPhysicalArmor}");
    }
}