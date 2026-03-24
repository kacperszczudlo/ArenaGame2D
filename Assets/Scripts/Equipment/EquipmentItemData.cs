using UnityEngine;

public enum ItemType
{
    Any, Weapon, Helmet, Armor, Pants, Boots, Ring, Gloves, Belt, Cape, Necklace
}

[CreateAssetMenu(fileName = "New Equipment", menuName = "Arena2D/Equipment Item")]
public class EquipmentItemData : ScriptableObject
{
    [Header("Ekonomia")]
    public int sellPrice; // <--- CENA SPRZEDAŻY
    public string itemName;
    public ItemType itemType;
    public Sprite icon;
    public string iconName; 
    
    [Header("Bonusy do statystyk")]
    public int bonusStrength;
    public int bonusAgility;
    public int bonusKnowledge;
}