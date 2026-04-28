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
                    // --- NOWA BLOKADA STATYSTYK (Tylko przy zakładaniu na ciało) ---
                    if (this.isEquippedSlot && PlayerDataManager.Instance != null)
                    {
                        int pStr = PlayerDataManager.Instance.baseStrength;
                        int pAgi = PlayerDataManager.Instance.baseAgility;
                        int reqStr = draggableItem.itemData.requiredStrength;
                        int reqAgi = draggableItem.itemData.requiredAgility;

                        if (pStr < reqStr || pAgi < reqAgi)
                        {
                            Debug.LogWarning($"[BLOKADA] Brakuje statystyk! Wymagane: Siła {reqStr}, Zręczność {reqAgi}. Twoje (Bazowe): Siła {pStr}, Zręczność {pAgi}.");
                            return; // Przerywamy operację - item automatycznie wróci na swoje stare miejsce!
                        }
                    }

                    draggableItem.parentAfterDrag = transform;
                    draggableItem.transform.SetParent(transform, false);

                    Debug.Log($"[EKWIPUNEK] Sukces! Umieszczono {draggableItem.itemData.itemName} w slocie: {gameObject.name}");
                    
                    if (EquipmentStatsCalculator.Instance != null)
                    {
                        EquipmentStatsCalculator.Instance.RecalculateAllEquipmentStats();
                    }

                    if (InventorySaveSystem.Instance != null)
                    {
                        InventorySaveSystem.Instance.SaveInventory();
                    }
                    // Refresh mirrors in upgrade window so visuals stay in sync
                    if (UpgradeManager.Instance != null) UpgradeManager.Instance.RefreshMirrors();
                }
                else
                {
                    Debug.LogWarning($"[EKWIPUNEK] Odmowa! Slot {gameObject.name} jest już zajęty.");
                }
            }
            else
            {
                Debug.LogWarning($"[EKWIPUNEK] Odmowa! Zły typ przedmiotu.");
            }
        }
    }
}