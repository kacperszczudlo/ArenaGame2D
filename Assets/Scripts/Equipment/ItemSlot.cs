using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    [Header("Ustawienia Slotu")]
    public ItemType allowedType = ItemType.Any; 
    public bool isEquippedSlot = false; 

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        DraggableItem draggableItem = droppedObject.GetComponent<DraggableItem>();

        if (draggableItem != null)
        {
            if (allowedType == ItemType.Any || draggableItem.itemData.itemType == allowedType)
            {
                if (transform.childCount == 0)
                {
                    draggableItem.parentAfterDrag = transform;
                    
                    // --- KLUCZOWA ZMIANA ---
                    // Zmuszamy przedmiot do fizycznego wejścia w nowe okienko JUŻ TERAZ, 
                    // zanim kalkulator zacznie skanować ekwipunek!
                    draggableItem.transform.SetParent(transform, false);

                    Debug.Log($"[EKWIPUNEK] Sukces! Umieszczono {draggableItem.itemData.itemName} w slocie: {gameObject.name}");
                    
                    // Skoro zmieniliśmy układ (z plecaka na ciało lub odwrotnie), zmuszamy do przeliczenia całości
                    if (EquipmentStatsCalculator.Instance != null)
                    {
                        EquipmentStatsCalculator.Instance.RecalculateAllEquipmentStats();
                    }

                    // Automatycznie zapisujemy stan ekwipunku, żeby zmiana została na stałe
                    if (InventorySaveSystem.Instance != null)
                    {
                        InventorySaveSystem.Instance.SaveInventory();
                    }
                }
                else
                {
                    Debug.LogWarning($"[EKWIPUNEK] Odmowa! Slot {gameObject.name} jest już zajęty przez inny przedmiot.");
                }
            }
            else
            {
                Debug.LogWarning($"[EKWIPUNEK] Odmowa! Próbujesz włożyć przedmiot typu '{draggableItem.itemData.itemType}' do slotu przeznaczonego wyłącznie na '{allowedType}'.");
            }
        }
    }
}