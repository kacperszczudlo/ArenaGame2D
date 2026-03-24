using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemSlotUI : MonoBehaviour 
{
    private TMP_Text nameText;       
    private TMP_Text priceText;      
    private Image iconImage;         
    private TMP_Text greenStatsText; 
    private Button buyButton;

    private void Awake()
    {
        nameText = transform.Find("NameAndTier_Container/ItemText")?.GetComponent<TMP_Text>();
        iconImage = transform.Find("ItemIcon")?.GetComponent<Image>();
        greenStatsText = transform.Find("Stats_Container/Bonus")?.GetComponent<TMP_Text>();
        priceText = transform.Find("Buy_Container/Price")?.GetComponent<TMP_Text>();
        buyButton = transform.Find("Buy_Container/BuyBtn")?.GetComponent<Button>();
    }

    public void Setup(string fullName, ItemTier tier, ItemCategory category, ItemVariant variant, BlacksmithShopController shop) 
    {
        if (nameText != null) { 
            nameText.enableWordWrapping = false; 
            nameText.text = fullName + "\n<color=#b3b3b3><size=70%>TIER: " + tier.name + "</size></color>"; 
            nameText.color = tier.color; 
        }
        
        if (priceText != null) priceText.text = tier.price.ToString() + " g";
        if (iconImage != null) {
            if (category.icon != null) {
                iconImage.sprite = category.icon;
                iconImage.color = new Color(1f, 1f, 1f, 1f);
            } else {
                iconImage.color = new Color(1f, 1f, 1f, 0f); // Ukryj kwadrat
            }
        }
        if (greenStatsText != null && category.stats != null) {
            greenStatsText.enableWordWrapping = false; 
            string allStats = "";
            for(int i = 0; i < category.stats.Length; i++) {
                int val = Mathf.RoundToInt((tier.level * 10) * variant.multiplier / (i + 1));
                allStats += $"+{val} {category.stats[i]}\n";
            }
            greenStatsText.text = allStats;
            greenStatsText.color = new Color(0.2f, 0.8f, 0.2f); 
        }

        if (buyButton != null) 
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => 
            {
                // SPRAWDZAMY ZŁOTO W GAMEMANAGERZE
                if (GameManager.Instance != null && GameManager.Instance.globalGold >= tier.price)
                {
                    EquipmentItemData generatedItem = ScriptableObject.CreateInstance<EquipmentItemData>();
                    generatedItem.itemName = fullName;
                    generatedItem.iconName = category.iconName; 
                    generatedItem.icon = Resources.Load<Sprite>("BlacksmithShop/" + category.iconName);
                    
                    generatedItem.sellPrice = tier.price / 2; 

                    switch(category.name.ToUpper())
                    {
                        case "BROŃ": generatedItem.itemType = ItemType.Weapon; break;
                        case "HEŁM": generatedItem.itemType = ItemType.Helmet; break;
                        case "ZBROJA": generatedItem.itemType = ItemType.Armor; break;
                        case "SPODNIE": generatedItem.itemType = ItemType.Pants; break;
                        case "BUTY": generatedItem.itemType = ItemType.Boots; break;
                        case "PIERŚCIEŃ OBR":
                        case "PIERŚCIEŃ ATK": generatedItem.itemType = ItemType.Ring; break;
                        case "RĘKAWICZKI": generatedItem.itemType = ItemType.Gloves; break;
                        case "PASEK": generatedItem.itemType = ItemType.Belt; break;
                        case "PELERYNA": generatedItem.itemType = ItemType.Cape; break;
                        case "NASZYJNIK": generatedItem.itemType = ItemType.Necklace; break;
                        default: generatedItem.itemType = ItemType.Any; break;
                    }

                    // Wstrzykujemy statystyki od kowala prosto do pliku przedmiotu!
                    for(int i = 0; i < category.stats.Length; i++) {
                        int val = Mathf.RoundToInt((tier.level * 10) * variant.multiplier / (i + 1));
                        
                        switch (category.stats[i].ToUpper()) {
                            case "PANCERZ": generatedItem.bonusPhysicalArmor += val; break;
                            case "PUNKTY ŻYCIA": generatedItem.bonusMaxHP += val; break;
                            case "OBRAŻENIA": generatedItem.weaponDamage += val; break;
                            case "SIŁA": generatedItem.bonusStrength += val; break;
                            case "MANA": generatedItem.bonusMaxMana += val; break;
                            case "WYTRZYMAŁOŚĆ": generatedItem.bonusMaxStamina += val; break;
                            case "ODPORNOŚĆ": generatedItem.bonusMagicResistance += val; break;
                            case "KRYT": generatedItem.bonusCritChance += val; break;
                            case "UNIK": generatedItem.bonusDodgeChance += val; break;
                            case "INTELIGENCJA": 
                            case "WIEDZA": generatedItem.bonusKnowledge += val; break;
                            case "ZWINNOŚĆ":
                            case "MOBILNOŚĆ":
                            case "SZYBKOŚĆ":
                            case "SZYBKOŚĆ ATAKU": generatedItem.bonusAgility += val; break;
                        }
                    }

                    bool success = InventoryManager.Instance.AddItemToInventory(generatedItem);                    
                    if (success)
                    {
                        // WYDAJEMY ZŁOTO BEZPOŚREDNIO Z BANKU
                        GameManager.Instance.SpendGold(tier.price);
                        shop.UpdateGoldUI(); 
                    }
                }
            });
        }
    }
}