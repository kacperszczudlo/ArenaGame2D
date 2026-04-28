using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemSlotUI : MonoBehaviour 
{
    private TMP_Text nameText, priceText, greenStatsText;      
    private Image iconImage;         
    private Button buyButton;

    private int displayedPrice;

    private void Awake()
    {
        nameText = transform.Find("NameAndTier_Container/ItemText")?.GetComponent<TMP_Text>();
        iconImage = transform.Find("ItemIcon")?.GetComponent<Image>();
        greenStatsText = transform.Find("Stats_Container/Bonus")?.GetComponent<TMP_Text>();
        priceText = transform.Find("Buy_Container/Price")?.GetComponent<TMP_Text>();
        buyButton = transform.Find("Buy_Container/BuyBtn")?.GetComponent<Button>();
    }

    public void Setup(ItemCategory category, ShopTier tier, int variantIndex, string suffix, BlacksmithShopController shop) 
    {
        string fullName = $"{category.name} {suffix.ToUpper()}";

        if (nameText != null) { 
            nameText.textWrappingMode = TextWrappingModes.NoWrap; 
            // Skalujemy wymaganą statystykę: Lvl 35 to wymóg 50 Siły/Zwinności (współczynnik 1.42)
            int reqStat = Mathf.RoundToInt(tier.level * 1.428f);
            string reqTxt = variantIndex == 2 ? $"Wymaga: {reqStat} Zręczności" : $"Wymaga: {reqStat} Siły";
            nameText.text = fullName + $"\n<color=#b3b3b3><size=70%>LVL: {tier.level} | {reqTxt}</size></color>"; 
        }
        
        if (priceText != null) priceText.text = tier.price.ToString() + " g";
        displayedPrice = shop != null ? shop.GetShopPrice(tier) : Mathf.Max(1, Mathf.RoundToInt(tier.price / 2f));
        if (priceText != null) priceText.text = displayedPrice.ToString() + " g";
        
        if (iconImage != null) {
            if (category.icon != null) { iconImage.sprite = category.icon; iconImage.color = Color.white; } 
            else iconImage.color = new Color(1f, 1f, 1f, 0f);
        }

        // --- WYLICZANIE STATYSTYK BAZOWYCH ---
        int armorFiz = 0, armorMag = 0, dmg = 0;
        bool isArmorType = (category.name == "HEŁM" || category.name == "ZBROJA" || category.name == "SPODNIE" || category.name == "BUTY");
        
        if (isArmorType) { armorFiz = tier.maxArmor; armorMag = tier.maxArmor; }
        if (category.name == "BROŃ") { dmg = tier.weaponDamage; }

        // --- WYLICZANIE WARIANTU (Tylko 1 statystyka bonusowa na item!) ---
        int hp = 0, str = 0, agi = 0, sta = 0, mana = 0;
        switch (variantIndex) {
            case 0: hp = tier.maxStats * 10; break;     // "Życia"
            case 1: str = tier.maxStats; break;         // "Siły"
            case 2: agi = tier.maxStats; break;         // "Zwinności"
            case 3: sta = tier.maxStats * 10; break;    // "Kondycji"
            case 4: mana = tier.maxStats * 10; break;   // "Many"
        }

        // --- BUDOWANIE TEKSTU DLA GRACZA ---
        if (greenStatsText != null) {
            greenStatsText.textWrappingMode = TextWrappingModes.NoWrap; 
            string allStats = "";
            if (dmg > 0) allStats += $"+{dmg} Obrażeń\n";
            if (armorFiz > 0) allStats += $"+{armorFiz} Pancerza Fiz/Mag\n";
            if (hp > 0) allStats += $"+{hp} Punkty Życia\n";
            if (str > 0) allStats += $"+{str} Siły\n";
            if (agi > 0) allStats += $"+{agi} Zręczności\n";
            if (sta > 0) allStats += $"+{sta} Kondycji\n";
            if (mana > 0) allStats += $"+{mana} Many\n";
            
            greenStatsText.text = allStats;
            greenStatsText.color = new Color(0.2f, 0.8f, 0.2f); 
        }

        if (buyButton != null) 
        {
            if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.currentLevel < tier.level)
            {
                buyButton.interactable = false; // Wyszarzamy przycisk
                if (priceText != null) priceText.text = $"<color=red>LVL {tier.level}</color>";
            }
            else
            {
                buyButton.interactable = true; // Pozwalamy kupić
                if (priceText != null) priceText.text = displayedPrice.ToString() + " g";
            }
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => 
            {
                if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.currentLevel < tier.level) return;

                if (GameManager.Instance != null && GameManager.Instance.globalGold >= displayedPrice)
                {
                    EquipmentItemData generatedItem = ScriptableObject.CreateInstance<EquipmentItemData>();
                    generatedItem.itemName = fullName;
                    generatedItem.iconName = category.iconName; 
                    generatedItem.icon = Resources.Load<Sprite>("BlacksmithShop/" + category.iconName);
                    generatedItem.sellPrice = Mathf.Max(1, Mathf.RoundToInt(displayedPrice / 2f)); 
                    generatedItem.requiredLevel = tier.level;
                    
                    // Wymagania
                    if (variantIndex == 2) generatedItem.requiredAgility = Mathf.RoundToInt(tier.level * 1.428f);
                    else generatedItem.requiredStrength = Mathf.RoundToInt(tier.level * 1.428f);

                    // Pancerz i Broń
                    generatedItem.bonusPhysicalArmor = armorFiz;
                    generatedItem.bonusMagicResistance = armorMag;
                    generatedItem.weaponDamage = dmg;

                    // Warianty
                    generatedItem.bonusMaxHP = hp;
                    generatedItem.bonusStrength = str;
                    generatedItem.bonusAgility = agi;
                    generatedItem.bonusMaxStamina = sta;
                    generatedItem.bonusMaxMana = mana;

                    // Typ itemu
                    switch(category.name.ToUpper()) {
                        case "BROŃ": generatedItem.itemType = ItemType.Weapon; break;
                        case "HEŁM": generatedItem.itemType = ItemType.Helmet; break;
                        case "ZBROJA": generatedItem.itemType = ItemType.Armor; break;
                        case "SPODNIE": generatedItem.itemType = ItemType.Pants; break;
                        case "BUTY": generatedItem.itemType = ItemType.Boots; break;
                        case "PIERŚCIEŃ": generatedItem.itemType = ItemType.Ring; break;
                        case "RĘKAWICZKI": generatedItem.itemType = ItemType.Gloves; break;
                        case "PASEK": generatedItem.itemType = ItemType.Belt; break;
                        case "PELERYNA": generatedItem.itemType = ItemType.Cape; break;
                        case "NASZYJNIK": generatedItem.itemType = ItemType.Necklace; break;
                        default: generatedItem.itemType = ItemType.Any; break;
                    }

                    if (InventoryManager.Instance.AddItemToInventory(generatedItem)) {
                        GameManager.Instance.SpendGold(displayedPrice);
                        shop.UpdateGoldUI(); 
                    }
                }
            });
        }
    }
}