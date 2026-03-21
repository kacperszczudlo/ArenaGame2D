using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance; 

    public Transform gridInventory; 
    private InventorySlot[] allSlots;

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
        }
    }

    void Start()
    {
        if (gridInventory != null) {
            List<InventorySlot> tempSlots = new List<InventorySlot>();
            
            foreach (Transform child in gridInventory) {
                InventorySlot s = child.GetComponent<InventorySlot>();
                if (s != null) {
                    tempSlots.Add(s);
                }
            }
            
            allSlots = tempSlots.ToArray();
            Debug.Log($"Zainicjowano {allSlots.Length} slotów ekwipunku.");
        } else {
            Debug.LogError("UWAGA: Nie przypisałeś Grid_Inventory w Inspektorze!");
        }
    }

    public bool AddItemToInventory(Sprite icon, string categoryName)
    {
        if (allSlots == null || allSlots.Length == 0) {
            Debug.LogError("Brak slotów! Sprawdź czy wrzuciłeś sloty do Grid_Inventory.");
            return false;
        }

        foreach (InventorySlot slot in allSlots)
        {
            if (!slot.isFull) 
            {
                slot.AddItem(icon, categoryName);
                return true; 
            }
        }
        
        Debug.Log("Ekwipunek jest pełny!");
        return false; 
    }
}