using UnityEngine;

public enum ItemType
{
    Any, Weapon, Helmet, Armor, Pants, Boots, Ring, Gloves, Belt, Cape, Necklace
}

[CreateAssetMenu(fileName = "New Equipment", menuName = "Arena2D/Equipment Item")]
public class EquipmentItemData : ScriptableObject
{
    [Header("Informacje Podstawowe")]
    public string itemName;
    public ItemType itemType;
    public Sprite icon;
    public string iconName; 
    
    [Header("Ekonomia")]
    public int sellPrice; 
    // Usunięto zduplikowany requiredLevel stąd

    [Header("Statystyki (Zgodne z PlayerDataManager)")]
    public int bonusMaxHP;
    public int bonusMaxMana;
    public int bonusMaxStamina;

    public int bonusStrength;
    public int bonusAgility;
    public int bonusKnowledge;
    public int bonusPower;

    public int bonusPhysicalArmor;
    public int bonusMagicResistance;
    
    [Header("Tylko dla Broni")]
    public int weaponDamage;

    [Header("Unikalne Bonusy (Zaklinacz)")]
    public int bonusCritChance;
    public int bonusDodgeChance;
    public float bonusDamageMultiplier; 
    public float bonusHitChanceMultiplier;

    [Header("Wymagania do założenia")]
    public int requiredLevel;
    public int requiredStrength;
    public int requiredAgility;
}