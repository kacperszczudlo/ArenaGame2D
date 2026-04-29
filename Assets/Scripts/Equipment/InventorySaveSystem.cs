using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SavedItem
{
    public string itemName;
    public int itemType;
    public string iconName;
    public string slotName; // full hierarchy path of the parent slot

    public int b_HP, b_Mana, b_Stamina;
    public int b_Str, b_Agi, b_Know, b_Pow;
    public int b_Armor, b_Resist, w_Damage;
    public int b_Crit, b_Dodge;
    public float b_DmgMult, b_HitMult;

    public int sellPrice;
    public int upgradeLevel; // per-instance upgrade level
    public int[] upgradePoints;
}

[System.Serializable]
public class InventorySaveData
{
    public List<SavedItem> savedItems = new List<SavedItem>();
}

public class InventorySaveSystem : MonoBehaviour
{
    public static InventorySaveSystem Instance;
    private const string SAVE_KEY = "Arena2D_InventorySave";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        LoadInventory();
    }

    private static string GetTransformPath(Transform target)
    {
        if (target == null) return string.Empty;

        List<string> parts = new List<string>();
        Transform current = target;
        while (current != null)
        {
            parts.Add(current.name);
            current = current.parent;
        }

        parts.Reverse();
        return string.Join("/", parts);
    }

    public void SaveInventory()
    {
        InventorySaveData saveData = new InventorySaveData();

        DraggableItem[] allItems = FindObjectsByType<DraggableItem>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (DraggableItem item in allItems)
        {
            // skip mirror visuals (they are not real inventory items)
            if (item == null || item.isMirror) continue;
            if (item.itemData == null || item.transform.parent == null) continue;

            // Only save items that are inside a real ItemSlot.
            // Upgrade window drop zones are not part of the persistent inventory state.
            ItemSlot parentSlot = item.transform.parent.GetComponent<ItemSlot>();
            if (parentSlot == null)
            {
                continue;
            }

            SavedItem sItem = new SavedItem();
            sItem.itemName = item.itemData.itemName;
            sItem.itemType = (int)item.itemData.itemType;
            sItem.iconName = item.itemData.iconName;
            sItem.slotName = GetTransformPath(item.transform.parent); 

            sItem.b_HP = item.itemData.bonusMaxHP; sItem.b_Mana = item.itemData.bonusMaxMana; sItem.b_Stamina = item.itemData.bonusMaxStamina;
            sItem.b_Str = item.itemData.bonusStrength; sItem.b_Agi = item.itemData.bonusAgility; sItem.b_Know = item.itemData.bonusKnowledge; sItem.b_Pow = item.itemData.bonusPower;
            sItem.b_Armor = item.itemData.bonusPhysicalArmor; sItem.b_Resist = item.itemData.bonusMagicResistance; sItem.w_Damage = item.itemData.weaponDamage;
            sItem.b_Crit = item.itemData.bonusCritChance; sItem.b_Dodge = item.itemData.bonusDodgeChance;
            sItem.b_DmgMult = item.itemData.bonusDamageMultiplier; sItem.b_HitMult = item.itemData.bonusHitChanceMultiplier;

            sItem.sellPrice = item.itemData.sellPrice;
            sItem.upgradeLevel = item.upgradeLevel;
            sItem.upgradePoints = item.upgradePoints != null ? item.upgradePoints.ToArray() : null;

            saveData.savedItems.Add(sItem);
        }

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();

        Debug.Log($"[ZAPIS] Zapisano ekwipunek! Znaleziono {saveData.savedItems.Count} przedmiotów.");
    }

    public void LoadInventory()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY)) return;

        string json = PlayerPrefs.GetString(SAVE_KEY);
        InventorySaveData loadData = JsonUtility.FromJson<InventorySaveData>(json);

        DraggableItem[] existingItems = FindObjectsByType<DraggableItem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var itm in existingItems) Destroy(itm.gameObject);

        // --- NAPRAWA: Szukamy wszystkich slotów na scenie, NAWET TYCH WYŁĄCZONYCH ---
        ItemSlot[] allSlotsOnScene = FindObjectsByType<ItemSlot>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (SavedItem sItem in loadData.savedItems)
        {
            EquipmentItemData restoredData = ScriptableObject.CreateInstance<EquipmentItemData>();
            restoredData.itemName = sItem.itemName;
            restoredData.itemType = (ItemType)sItem.itemType;
            restoredData.iconName = sItem.iconName;

            restoredData.bonusMaxHP = sItem.b_HP; restoredData.bonusMaxMana = sItem.b_Mana; restoredData.bonusMaxStamina = sItem.b_Stamina;
            restoredData.bonusStrength = sItem.b_Str; restoredData.bonusAgility = sItem.b_Agi; restoredData.bonusKnowledge = sItem.b_Know; restoredData.bonusPower = sItem.b_Pow;
            restoredData.bonusPhysicalArmor = sItem.b_Armor; restoredData.bonusMagicResistance = sItem.b_Resist; restoredData.weaponDamage = sItem.w_Damage;
            restoredData.bonusCritChance = sItem.b_Crit; restoredData.bonusDodgeChance = sItem.b_Dodge;
            restoredData.bonusDamageMultiplier = sItem.b_DmgMult; restoredData.bonusHitChanceMultiplier = sItem.b_HitMult;

            restoredData.sellPrice = sItem.sellPrice;
            // create instance will get upgrade level applied to spawned DraggableItem
            
            // Ładowanie grafiki z folderu Resources
            restoredData.icon = Resources.Load<Sprite>("BlacksmithShop/" + sItem.iconName);

            // Szukamy slota w naszej pobranej liście
            ItemSlot targetSlot = null;
            foreach (ItemSlot slot in allSlotsOnScene)
            {
                string slotPath = GetTransformPath(slot.transform);
                if (slotPath == sItem.slotName || slot.gameObject.name == sItem.slotName)
                {
                    targetSlot = slot;
                    break;
                }
            }
            
            if (targetSlot != null && InventoryManager.Instance != null && InventoryManager.Instance.draggableItemPrefab != null)
            {
                GameObject spawnedItem = Instantiate(InventoryManager.Instance.draggableItemPrefab, targetSlot.transform);
                
                RectTransform rect = spawnedItem.GetComponent<RectTransform>();
                rect.localScale = Vector3.one; 
                rect.anchorMin = new Vector2(0, 0); 
                rect.anchorMax = new Vector2(1, 1); 
                rect.offsetMin = Vector2.zero; 
                rect.offsetMax = Vector2.zero;
                rect.anchoredPosition = Vector2.zero; 

                DraggableItem dragLogic = spawnedItem.GetComponent<DraggableItem>();
                if(dragLogic != null) { dragLogic.Setup(restoredData); dragLogic.upgradeLevel = sItem.upgradeLevel; if (sItem.upgradePoints != null) { dragLogic.upgradePoints = new System.Collections.Generic.List<int>(sItem.upgradePoints); } }
            }
            else
            {
                Debug.LogWarning($"[WCZYTYWANIE BŁĄD] Nie znalazłem ukrytego slota o nazwie '{sItem.slotName}'!");
            }
        } // <--- Koniec głównej pętli foreach
        
        // --- NOWY KOD: Podliczanie statystyk po całkowitym załadowaniu ekwipunku ---
        if (EquipmentStatsCalculator.Instance != null)
        {
            EquipmentStatsCalculator.Instance.RecalculateAllEquipmentStats();
            Debug.Log("[WCZYTYWANIE] EquipmentStatsCalculator znaleziony - przeliczanie bonusów.");
        }
        else
        {
            // Fallback: Spróbuj znaleźć EquipmentStatsCalculator jeśli Singleton nie został jeszcze ustawiony
            EquipmentStatsCalculator calc = FindFirstObjectByType<EquipmentStatsCalculator>();
            if (calc != null)
            {
                calc.RecalculateAllEquipmentStats();
                Debug.Log("[WCZYTYWANIE] EquipmentStatsCalculator znaleziony przez FindFirst - przeliczanie bonusów.");
            }
            else
            {
                Debug.LogWarning("[WCZYTYWANIE] EquipmentStatsCalculator NIE znaleziony! Bonusy mogą nie być przeliczone. Spróbuję ponownie za 1 frame.");
                // Spróbuj jeszcze raz za frame'a
                StartCoroutine(DeferredRecalculateStats());
            }
        }

        Debug.Log($"[WCZYTYWANIE] Wczytano {loadData.savedItems.Count} przedmiotów z zapisu.");
    }

    private System.Collections.IEnumerator DeferredRecalculateStats()
    {
        yield return null; // Wait one frame
        if (EquipmentStatsCalculator.Instance != null)
        {
            EquipmentStatsCalculator.Instance.RecalculateAllEquipmentStats();
            Debug.Log("[WCZYTYWANIE] Deferred: EquipmentStatsCalculator znaleziony - przeliczanie bonusów.");
        }
        else
        {
            EquipmentStatsCalculator calc = FindFirstObjectByType<EquipmentStatsCalculator>();
            if (calc != null)
            {
                calc.RecalculateAllEquipmentStats();
                Debug.Log("[WCZYTYWANIE] Deferred: EquipmentStatsCalculator znaleziony przez FindFirst - przeliczanie bonusów.");
            }
            else
            {
                Debug.LogError("[WCZYTYWANIE] KRYTYCZNIE: EquipmentStatsCalculator nie istnieje na scenie!");
            }
        }
    }
    
    private void OnApplicationQuit()
    {
        SaveInventory();
    }
}