using UnityEngine;
using UnityEngine.UI;

public class InventoryFilter : MonoBehaviour
{
    [Header("Podepnij siatkę ze slotami plecaka")]
    public Transform gridInventory; 

    // Funkcję przypisujesz do przycisków w Inspektorze (On Click) wpisując w polu tekstowym argument (np. "Bron")
    public void FilterByCategory(string categoryName)
    {
        if (gridInventory == null) return;

        foreach (Transform slot in gridInventory)
        {
            if (slot.childCount > 0) 
            {
                DraggableItem item = slot.GetChild(0).GetComponent<DraggableItem>();
                if (item != null && item.itemData != null)
                {
                    bool shouldShow = false;

                    switch (categoryName)
                    {
                        case "Wszystko": 
                            shouldShow = true; 
                            break;
                        case "Bron": 
                            shouldShow = (item.itemData.itemType == ItemType.Weapon); 
                            break;
                        case "Pancerz": 
                            shouldShow = (item.itemData.itemType == ItemType.Armor || 
                                          item.itemData.itemType == ItemType.Helmet || 
                                          item.itemData.itemType == ItemType.Boots || 
                                          item.itemData.itemType == ItemType.Pants || 
                                          item.itemData.itemType == ItemType.Gloves || 
                                          item.itemData.itemType == ItemType.Cape); 
                            break;
                        case "Bizuteria": 
                            shouldShow = (item.itemData.itemType == ItemType.Ring || 
                                          item.itemData.itemType == ItemType.Necklace || 
                                          item.itemData.itemType == ItemType.Belt); 
                            break;
                        case "Qests":
                            // Zostawiam na przyszłość
                            shouldShow = false; 
                            break;
                    }

                    item.gameObject.SetActive(shouldShow);
                }
            }
        }
    }
}