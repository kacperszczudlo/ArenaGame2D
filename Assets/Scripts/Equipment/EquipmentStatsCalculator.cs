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

                    // Apply per-item upgrade points (if any)
                    if (item.upgradePoints != null)
                    {
                        // index mapping: 0=Brak,1=HP,2=Str,3=Agi,4=Sta,5=Mana,6=PhysArmor,7=MagRes,8=WeapDmg,9=Crit,10=Dodge,11=DmgMult,12=HitMult
                        int pts = 0;
                        // HP
                        if (item.upgradePoints.Count > 1) { pts = item.upgradePoints[1]; p.bonusMaxHP += pts * 10; }
                        // Strength
                        if (item.upgradePoints.Count > 2) { pts = item.upgradePoints[2]; p.bonusStrength += pts; }
                        // Agility
                        if (item.upgradePoints.Count > 3) { pts = item.upgradePoints[3]; p.bonusAgility += pts; }
                        // Stamina
                        if (item.upgradePoints.Count > 4) { pts = item.upgradePoints[4]; p.bonusMaxStamina += pts * 10; }
                        // Mana
                        if (item.upgradePoints.Count > 5) { pts = item.upgradePoints[5]; p.bonusMaxMana += pts * 10; }
                        // Physical Armor
                        if (item.upgradePoints.Count > 6) { pts = item.upgradePoints[6]; p.bonusPhysicalArmor += pts; }
                        // Magic Resist
                        if (item.upgradePoints.Count > 7) { pts = item.upgradePoints[7]; p.bonusMagicResistance += pts; }
                        // Weapon Damage
                        if (item.upgradePoints.Count > 8) { pts = item.upgradePoints[8]; p.weaponDamage += pts; }
                        // Crit Chance
                        if (item.upgradePoints.Count > 9) { pts = item.upgradePoints[9]; p.bonusCritChance += pts; }
                        // Dodge Chance
                        if (item.upgradePoints.Count > 10) { pts = item.upgradePoints[10]; p.bonusDodgeChance += pts; }
                        // Damage Multiplier
                        if (item.upgradePoints.Count > 11) { pts = item.upgradePoints[11]; p.bonusDamageMultiplier += pts * 0.05f; }
                        // Hit Chance Multiplier
                        if (item.upgradePoints.Count > 12) { pts = item.upgradePoints[12]; p.bonusHitChanceMultiplier += pts * 0.05f; }
                    }
                }
            }
        }
        
        Debug.Log($"[KALKULATOR] Podliczono nowy ekwipunek! Całkowita dodatkowa siła: {p.bonusStrength}, Pancerz fiz.: {p.bonusPhysicalArmor}");
        if (PlayerStatsUI.Instance != null) PlayerStatsUI.Instance.UpdateStatsUI();
    }
}