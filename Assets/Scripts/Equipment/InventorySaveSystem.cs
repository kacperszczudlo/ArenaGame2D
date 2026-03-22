using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SavedItem
{
    public string itemName;
    public int itemType;
    public string iconName;
    public string slotName; 
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

    public void SaveInventory()
    {
        InventorySaveData saveData = new InventorySaveData();

        DraggableItem[] allItems = FindObjectsByType<DraggableItem>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (DraggableItem item in allItems)
        {
            if (item.itemData == null || item.transform.parent == null) continue;

            SavedItem sItem = new SavedItem();
            sItem.itemName = item.itemData.itemName;
            sItem.itemType = (int)item.itemData.itemType;
            sItem.iconName = item.itemData.iconName;
            sItem.slotName = item.transform.parent.name; 

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
            
            // Ładowanie grafiki z folderu Resources
            restoredData.icon = Resources.Load<Sprite>("BlacksmithShop/" + sItem.iconName);

            // Szukamy slota w naszej pobranej liście
            ItemSlot targetSlot = null;
            foreach (ItemSlot slot in allSlotsOnScene)
            {
                if (slot.gameObject.name == sItem.slotName)
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
                if(dragLogic != null) dragLogic.Setup(restoredData);
            }
            else
            {
                Debug.LogWarning($"[WCZYTYWANIE BŁĄD] Nie znalazłem ukrytego slota o nazwie '{sItem.slotName}'!");
            }
        }
        Debug.Log($"[WCZYTYWANIE] Wczytano {loadData.savedItems.Count} przedmiotów z zapisu.");
    }
    
    private void OnApplicationQuit()
    {
        SaveInventory();
    }
}