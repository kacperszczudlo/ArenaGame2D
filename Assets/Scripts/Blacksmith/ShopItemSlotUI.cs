using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemSlotUI : MonoBehaviour 
{
    [Header("Podepnij z Prefaba:")]
    public TMP_Text nameText;       
    public TMP_Text priceText;      
    public Image iconImage;         
    public TMP_Text greenStatsText; 
    public Button buyButton;

    public void Setup(string fullName, ItemTier tier, ItemCategory category, ItemVariant variant, BlacksmithShopController shop) 
    {
        if (nameText != null) { 
            nameText.enableWordWrapping = false; 
            nameText.text = fullName + "\n<color=#b3b3b3><size=70%>TIER: " + tier.name + "</size></color>"; 
            nameText.color = tier.color; 
        }
        
        if (priceText != null) priceText.text = tier.price.ToString() + " g";
        if (iconImage != null && category.icon != null) iconImage.sprite = category.icon;

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

        if (buyButton != null) {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => shop.TryBuyItem(tier.price, category.icon, category.name));
        }
    }
}