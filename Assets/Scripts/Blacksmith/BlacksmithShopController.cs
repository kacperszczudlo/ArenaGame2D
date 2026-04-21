using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable] public class ItemCategory { public string name; public string iconName; public Sprite icon; }
[Serializable] public class ShopTier { public int level; public int maxArmor; public int maxStats; public int weaponDamage; public int price; }

public class BlacksmithShopController : MonoBehaviour 
{
    [Header("Podepnij z folderu Prefabs:")]
    public GameObject categoryPrefab;
    public GameObject itemPrefab;

    private Button closeButton, prevTierButton, nextTierButton;    
    private TMP_Text pageText, goldTxt, titleTxt; 

    private List<ItemCategory> categories;
    private List<ShopTier> tiers;
    private string[] variantNames = { "Życia", "Siły", "Zwinności", "Kondycji", "Many" };

    private GameObject catView, itmView;
    private Transform catGrid, itmList;
    private int currentTierIndex = 0;
    private ItemCategory currentCategory;

    void Awake() 
    {
        Transform cv = transform.Find("Categories_View");
        if (cv != null) { catView = cv.gameObject; catGrid = cv.Find("Category_Grid"); }
        
        Transform iv = transform.Find("Items_View");
        if (iv != null) { itmView = iv.gameObject; itmList = iv.Find("Items_List"); titleTxt = iv.Find("CategoryName_Text")?.GetComponent<TMP_Text>(); }
        
        goldTxt = transform.Find("Header_Panel/Gold_Container/Gold_Text")?.GetComponent<TMP_Text>();

        transform.Find("Header_Panel/Back_Button")?.GetComponent<Button>().onClick.AddListener(ShowCategories);
        transform.Find("Header_Panel/CloseButton")?.GetComponent<Button>().onClick.AddListener(CloseShop);
        
        prevTierButton = transform.Find("Items_View/Pagination_Panel/Btn_Prev")?.GetComponent<Button>();
        if (prevTierButton != null) prevTierButton.onClick.AddListener(PreviousTier);

        nextTierButton = transform.Find("Items_View/Pagination_Panel/Btn_Next")?.GetComponent<Button>();
        if (nextTierButton != null) nextTierButton.onClick.AddListener(NextTier);

        pageText = transform.Find("Items_View/Pagination_Panel/Btn_Page_1/Text (TMP)")?.GetComponent<TMP_Text>() ?? 
                   transform.Find("Items_View/Pagination_Panel/Btn_Page_1")?.GetComponent<TMP_Text>();

        LoadDatabase();
    }

    void Start() { ShowCategories(); GenerateCategories(); UpdateGoldUI(); }

    public void UpdateGoldUI() {
        if (goldTxt != null && GameManager.Instance != null) 
            goldTxt.text = "ZŁOTO: " + GameManager.Instance.globalGold;
    }

    public void ShowCategories() {
        if (catView != null) catView.SetActive(true); 
        if (itmView != null) itmView.SetActive(false);
        transform.Find("Header_Panel/Back_Button")?.gameObject.SetActive(false);
    }

    public void OpenShop() { gameObject.SetActive(true); ShowCategories(); UpdateGoldUI(); }
    public void CloseShop() { gameObject.SetActive(false); }

    void GenerateCategories() {
        foreach (Transform child in catGrid) Destroy(child.gameObject);
        foreach (var c in categories) {
            var go = Instantiate(categoryPrefab, catGrid);
            go.GetComponentInChildren<TMP_Text>().text = c.name;
            var img = go.transform.Find("Icon")?.GetComponent<Image>() ?? go.GetComponentInChildren<Image>();
            if (img != null && c.icon != null) img.sprite = c.icon;
            go.GetComponent<Button>().onClick.AddListener(() => OnCategoryPick(c));
        }
    }

    void OnCategoryPick(ItemCategory c) {
        currentCategory = c;
        currentTierIndex = 0; 
        if (catView != null) catView.SetActive(false); 
        if (itmView != null) itmView.SetActive(true);
        transform.Find("Header_Panel/Back_Button")?.gameObject.SetActive(true);
        if (titleTxt != null) titleTxt.text = "KATEGORIA: " + c.name;
        RefreshItemsList();
    }

    void RefreshItemsList() {
        foreach (Transform child in itmList) Destroy(child.gameObject);

        ShopTier currentTier = tiers[currentTierIndex];

        // Zawsze generujemy dokładnie 5 wariantów dla danej strony
        for (int i = 0; i < 5; i++) {
            var go = Instantiate(itemPrefab, itmList);
            var slot = go.GetComponent<ShopItemSlotUI>() ?? go.AddComponent<ShopItemSlotUI>();
            slot.Setup(currentCategory, currentTier, i, variantNames[i], this); 
        }

        if (pageText != null) pageText.text = "LVL: " + currentTier.level; 
        if (prevTierButton != null) prevTierButton.interactable = (currentTierIndex > 0);
        if (nextTierButton != null) nextTierButton.interactable = (currentTierIndex < tiers.Count - 1);
    }

    public int GetShopPrice(ShopTier tier)
    {
        if (tier == null)
        {
            return 0;
        }

        return Mathf.Max(1, Mathf.RoundToInt(tier.price / 2f));
    }

    public void NextTier() { if (currentTierIndex < tiers.Count - 1) { currentTierIndex++; RefreshItemsList(); } }
    public void PreviousTier() { if (currentTierIndex > 0) { currentTierIndex--; RefreshItemsList(); } }

    void LoadDatabase() {
        // --- TABELKA OD NORBERTA NA SZTYWNO ---
        tiers = new List<ShopTier> {
            new ShopTier { level = 1, maxArmor = 0, maxStats = 1, weaponDamage = 20, price = 100 },
            new ShopTier { level = 3, maxArmor = 1, maxStats = 3, weaponDamage = 26, price = 250 },
            new ShopTier { level = 6, maxArmor = 2, maxStats = 6, weaponDamage = 35, price = 500 },
            new ShopTier { level = 9, maxArmor = 3, maxStats = 9, weaponDamage = 44, price = 900 },
            new ShopTier { level = 12, maxArmor = 4, maxStats = 12, weaponDamage = 53, price = 1400 },
            new ShopTier { level = 15, maxArmor = 5, maxStats = 15, weaponDamage = 62, price = 2000 },
            new ShopTier { level = 18, maxArmor = 6, maxStats = 18, weaponDamage = 71, price = 2800 },
            new ShopTier { level = 21, maxArmor = 7, maxStats = 21, weaponDamage = 80, price = 3800 },
            new ShopTier { level = 25, maxArmor = 8, maxStats = 25, weaponDamage = 92, price = 5000 },
            new ShopTier { level = 30, maxArmor = 10, maxStats = 30, weaponDamage = 107, price = 7500 },
            new ShopTier { level = 35, maxArmor = 25, maxStats = 35, weaponDamage = 140, price = 12000 }
        };

        categories = new List<ItemCategory> {
            new ItemCategory { name = "BROŃ", iconName = "game-icons_crossed-swords" },
            new ItemCategory { name = "HEŁM", iconName = "game-icons_visored-helm" },
            new ItemCategory { name = "ZBROJA", iconName = "game-icons_breastplate" },
            new ItemCategory { name = "SPODNIE", iconName = "game-icons_trousers" },
            new ItemCategory { name = "BUTY", iconName = "game-icons_leather-boot" },
            new ItemCategory { name = "PIERŚCIEŃ", iconName = "game-icons_ring" },
            new ItemCategory { name = "RĘKAWICZKI", iconName = "game-icons_gauntlet" },
            new ItemCategory { name = "PASEK", iconName = "game-icons_belt" },
            new ItemCategory { name = "PELERYNA", iconName = "game-icons_cape" },
            new ItemCategory { name = "NASZYJNIK", iconName = "game-icons_necklace" }
        };

        foreach(var cat in categories) cat.icon = Resources.Load<Sprite>("BlacksmithShop/" + cat.iconName);
    }
}