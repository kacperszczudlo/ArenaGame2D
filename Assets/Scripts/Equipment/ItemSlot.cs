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
            // Weryfikacja typu
            if (allowedType == ItemType.Any || draggableItem.itemData.itemType == allowedType)
            {
                // Weryfikacja zajętości
                if (transform.childCount == 0)
                {
                    draggableItem.parentAfterDrag = transform;
                    Debug.Log($"[EKWIPUNEK] Sukces! Umieszczono {draggableItem.itemData.itemName} w slocie: {gameObject.name}");
                    
                    UpdatePlayerStats(draggableItem.itemData, isEquippedSlot);
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

    private void UpdatePlayerStats(EquipmentItemData item, bool isEquipping)
    {
        // To zaimplementujemy w następnym kroku z udziałem Twojego PlayerDataManagera!
        if (isEquipping) Debug.Log($"Doliczam bonusy ze sprzętu: {item.itemName}");
        else Debug.Log($"Odejmuję bonusy ze sprzętu: {item.itemName}");
    }
}