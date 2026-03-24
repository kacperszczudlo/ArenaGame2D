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
                    ItemSlot oldSlot = draggableItem.parentAfterDrag.GetComponent<ItemSlot>();
                    
                    // Jeśli zdjęliśmy przedmiot z ciała - odejmujemy statystyki
                    if (oldSlot != null && oldSlot.isEquippedSlot) 
                    {
                        UpdatePlayerStats(draggableItem.itemData, false);
                    }

                    draggableItem.parentAfterDrag = transform;
                    Debug.Log($"[EKWIPUNEK] Sukces! Umieszczono {draggableItem.itemData.itemName} w slocie: {gameObject.name}");
                    
                    // Jeśli założyliśmy nowy przedmiot na ciało - dodajemy statystyki
                    if (this.isEquippedSlot)
                    {
                        UpdatePlayerStats(draggableItem.itemData, true);
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

    private void UpdatePlayerStats(EquipmentItemData item, bool isEquipping)
    {
        // Tutaj w przyszłości odwołasz się do PlayerDataManager
        if (isEquipping) 
        {
            Debug.Log($"[STATYSTYKI] ZAKŁADASZ sprzęt: {item.itemName}. Pora dodać buffy!");
        }
        else 
        {
            Debug.Log($"[STATYSTYKI] ZDEJMUJESZ sprzęt: {item.itemName}. Pora odjąć buffy!");
        }
    }
}