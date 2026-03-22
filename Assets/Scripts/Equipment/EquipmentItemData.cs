using UnityEngine;

public enum ItemType
{
    Any, Weapon, Helmet, Armor, Pants, Boots, Ring, Gloves, Belt, Cape, Necklace
}

[CreateAssetMenu(fileName = "New Equipment", menuName = "Arena2D/Equipment Item")]
public class EquipmentItemData : ScriptableObject
{
    public string itemName;
    public ItemType itemType;
    public Sprite icon;
    public string iconName; // [NOWE] Zapisuje nazwę obrazka, żeby gra mogła go wczytać z folderu Resources
    
    [Header("Bonusy do statystyk")]
    public int bonusStrength;
    public int bonusAgility;
    public int bonusKnowledge;
    // (Dodaj tu resztę statystyk z PlayerDataManagera, jeśli ich używasz)
}