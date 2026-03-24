using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable] public class ItemCategory { public string name; public string iconName; public string[] stats; public Sprite icon; }
[Serializable] public class ItemTier { public int level; public string name; public Color color; public int price; }
[Serializable] public class ItemVariant { public string suffix; public float multiplier; }

public class BlacksmithShopController : MonoBehaviour 
{
    [Header("Złoto i Ekonomia")]
    public int currentGold = 1500; 

    [Header("Podepnij z folderu Prefabs:")]
    public GameObject categoryPrefab;
    public GameObject itemPrefab;

    private Button closeButton;       
    private Button prevTierButton;    
    private Button nextTierButton;    
    private TMP_Text pageText; 

    private List<ItemCategory> categories;
    private List<ItemTier> tiers;
    private List<ItemVariant> variants;

    private GameObject catView, itmView;
    private Transform catGrid, itmList;
    private TMP_Text goldTxt, titleTxt;

    private int currentTierIndex = 0;
    private ItemCategory currentCategory;

    void Awake() 
    {
        Transform cv = transform.Find("Categories_View");
        if (cv != null) { catView = cv.gameObject; catGrid = cv.Find("Category_Grid"); }
        
        Transform iv = transform.Find("Items_View");
        if (iv != null) { itmView = iv.gameObject; itmList = iv.Find("Items_List"); titleTxt = iv.Find("CategoryName_Text")?.GetComponent<TMP_Text>(); }
        
        goldTxt = transform.Find("Header_Panel/Gold_Container/Gold_Text")?.GetComponent<TMP_Text>();

        Transform backBtn = transform.Find("Header_Panel/Back_Button");
        if (backBtn != null) backBtn.GetComponent<Button>().onClick.AddListener(ShowCategories);

        closeButton = transform.Find("Header_Panel/CloseButton")?.GetComponent<Button>();
        if (closeButton != null) closeButton.onClick.AddListener(CloseShop);

        prevTierButton = transform.Find("Items_View/Pagination_Panel/Btn_Prev")?.GetComponent<Button>();
        if (prevTierButton != null) prevTierButton.onClick.AddListener(PreviousTier);

        nextTierButton = transform.Find("Items_View/Pagination_Panel/Btn_Next")?.GetComponent<Button>();
        if (nextTierButton != null) nextTierButton.onClick.AddListener(NextTier);

        pageText = transform.Find("Items_View/Pagination_Panel/Btn_Page_1/Text (TMP)")?.GetComponent<TMP_Text>() ?? 
                   transform.Find("Items_View/Pagination_Panel/Btn_Page_1")?.GetComponent<TMP_Text>();

        LoadDatabase();
    }

    void Start() { 
        // Wczytujemy zapisane złoto po włączeniu gry (domyślnie 1500)
        currentGold = PlayerPrefs.GetInt("PlayerGold", 1500); 
        ShowCategories(); 
        GenerateCategories(); 
        UpdateGoldUI();
    }

    public void UpdateGoldUI() {
        if (goldTxt != null) goldTxt.text = "ZŁOTO: " + currentGold;
    }

    public void ShowCategories() {
        if (catView != null) catView.SetActive(true); 
        if (itmView != null) itmView.SetActive(false);
        transform.Find("Header_Panel/Back_Button")?.gameObject.SetActive(false);
    }

    public void OpenShop() { 
        gameObject.SetActive(true); 
        currentGold = PlayerPrefs.GetInt("PlayerGold", 1500); // Odśwież golda po otwarciu
        ShowCategories(); 
        UpdateGoldUI(); 
    }
    
    public void CloseShop() { gameObject.SetActive(false); }

    void GenerateCategories() {
        if (catGrid == null) return;
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
        if (itmList == null || currentCategory == null) return;
        foreach (Transform child in itmList) Destroy(child.gameObject);

        ItemTier currentTier = tiers[currentTierIndex];

        foreach (var v in variants) {
            var go = Instantiate(itemPrefab, itmList);
            var slot = go.GetComponent<ShopItemSlotUI>() ?? go.AddComponent<ShopItemSlotUI>();
            
            slot.Setup(currentCategory.name + " " + v.suffix, currentTier, currentCategory, v, this); 
        }

        if (pageText != null) pageText.text = (currentTierIndex + 1).ToString(); 
        if (prevTierButton != null) prevTierButton.interactable = (currentTierIndex > 0);
        if (nextTierButton != null) nextTierButton.interactable = (currentTierIndex < tiers.Count - 1);
    }

    public void NextTier() {
        if (currentTierIndex < tiers.Count - 1) { currentTierIndex++; RefreshItemsList(); }
    }

    public void PreviousTier() {
        if (currentTierIndex > 0) { currentTierIndex--; RefreshItemsList(); }
    }

    void LoadDatabase() {
        tiers = new List<ItemTier> {
            new ItemTier { level = 1, name = "ZWYKŁY", color = Color.white, price = 100 },
            new ItemTier { level = 2, name = "RZADKI", color = new Color(0.2f, 0.6f, 1f), price = 250 },
            new ItemTier { level = 3, name = "EPICKI", color = new Color(0.6f, 0.2f, 0.8f), price = 600 }
        };

        variants = new List<ItemVariant> {
            new ItemVariant { suffix = "ZNISZCZONY", multiplier = 0.5f },
            new ItemVariant { suffix = "ŻELAZNY", multiplier = 1.0f },
            new ItemVariant { suffix = "STALOWY", multiplier = 1.5f },
            new ItemVariant { suffix = "NIEDŹWIEDZIA", multiplier = 2.0f },
            new ItemVariant { suffix = "KRÓLEWSKI", multiplier = 3.0f }
        };

        categories = new List<ItemCategory> {
            new ItemCategory { name = "BROŃ", iconName = "game-icons_crossed-swords", stats = new string[]{"OBRAŻENIA", "SZYBKOŚĆ ATAKU"} },
            new ItemCategory { name = "HEŁM", iconName = "game-icons_visored-helm", stats = new string[]{"PANCERZ", "PUNKTY ŻYCIA"} },
            new ItemCategory { name = "ZBROJA", iconName = "game-icons_breastplate", stats = new string[]{"WYTRZYMAŁOŚĆ", "PANCERZ"} },
            new ItemCategory { name = "SPODNIE", iconName = "game-icons_trousers", stats = new string[]{"UNIK", "MOBILNOŚĆ"} },
            new ItemCategory { name = "BUTY", iconName = "game-icons_leather-boot", stats = new string[]{"SZYBKOŚĆ", "UNIK"} },
            new ItemCategory { name = "PIERŚCIEŃ OBR", iconName = "game-icons_diamond-ring", stats = new string[]{"ODPORNOŚĆ", "MANA"} },
            new ItemCategory { name = "PIERŚCIEŃ ATK", iconName = "game-icons_ring", stats = new string[]{"SIŁA", "KRYT"} },
            new ItemCategory { name = "RĘKAWICZKI", iconName = "game-icons_gauntlet", stats = new string[]{"SZYBKOŚĆ ATAKU", "ZWINNOŚĆ"} },
            new ItemCategory { name = "PASEK", iconName = "game-icons_belt", stats = new string[]{"PUNKTY ŻYCIA", "UDŹWIG"} },
            new ItemCategory { name = "PELERYNA", iconName = "game-icons_cape", stats = new string[]{"ODPORNOŚĆ", "CHARYZMA"} },
            new ItemCategory { name = "NASZYJNIK", iconName = "game-icons_necklace", stats = new string[]{"REGENERACJA", "INTELIGENCJA"} }
        };

        foreach(var cat in categories) {
            cat.icon = Resources.Load<Sprite>("BlacksmithShop/" + cat.iconName);
        }
    }
}