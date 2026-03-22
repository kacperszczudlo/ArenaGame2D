using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance; 

    [Header("Referencje")]
    public Transform gridInventory; 
    public GameObject draggableItemPrefab; 

    void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this); 
        }
    }

    public bool AddItemToInventory(EquipmentItemData newItemData)
    {
        // --- NOWE LOGI ŚLEDZĄCE ---
        Debug.Log($"[START] Menedżer na obiekcie: '{gameObject.name}' rozpoczyna działanie.");
        
        if (gridInventory != null) {
            Debug.Log($"[INFO] Patrzę w obiekt o nazwie '{gridInventory.name}'. Ten obiekt ma w sobie {gridInventory.childCount} elementów podrzędnych.");
        }
        // --------------------------

        if (draggableItemPrefab == null)
        {
            Debug.LogError($"BŁĄD: draggableItemPrefab jest pusty!");
            return false;
        }

        if (gridInventory == null) 
        {
            Debug.LogError("Brak referencji do Grid_Inventory!");
            return false;
        }

        int checkedSlots = 0;

        foreach (Transform slot in gridInventory)
        {
            checkedSlots++;
            if (slot.childCount == 0) 
            {
                GameObject spawnedItem = Instantiate(draggableItemPrefab, slot);
                
                // --- NAPRAWA NIEWIDZIALNOŚCI ---
                RectTransform rect = spawnedItem.GetComponent<RectTransform>();
                rect.localScale = Vector3.one; 
                
                // Zmuszamy obrazek do trybu "Stretch" (rozciągnij na 100% szerokości i wysokości rodzica)
                rect.anchorMin = new Vector2(0, 0); 
                rect.anchorMax = new Vector2(1, 1); 
                
                // Zerujemy marginesy, żeby idealnie wpasował się w krawędzie slota
                rect.offsetMin = Vector2.zero; 
                rect.offsetMax = Vector2.zero;

                DraggableItem dragLogic = spawnedItem.GetComponent<DraggableItem>();
                if(dragLogic != null) dragLogic.Setup(newItemData);

                Debug.Log($"Dodano {newItemData.itemName} do plecaka w slocie: {slot.name}!");
                return true; 
            }
            else
            {
                Debug.Log($"[DIAGNOSTYKA] Slot '{slot.name}' nie jest pusty! Blokuje go: '{slot.GetChild(0).name}'");
            }
        }
        
        Debug.LogWarning($"[DIAGNOSTYKA] Odmowa! Przeszukano dokładnie {checkedSlots} slotów i wszystkie są czymś zajęte.");
        return false; 
    }
}