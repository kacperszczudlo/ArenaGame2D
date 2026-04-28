using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeDropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
	public UpgradeManager manager; // optional reference to notify

	// The currently placed draggable (if any)
	public DraggableItem placedItem;

	public void OnDrop(PointerEventData eventData)
	{
		if (eventData.pointerDrag == null) return;

		DraggableItem draggable = eventData.pointerDrag.GetComponent<DraggableItem>();
		if (draggable == null) return;

		// Always operate on the original item when dragging from mirror view.
		if (draggable.isMirror && draggable.originalSource != null)
		{
			draggable = draggable.originalSource;
		}

		// Here you can filter item types if needed (e.g., only equipment)
		// For now accept only items with EquipmentItemData
		if (draggable.itemData == null)
		{
			Debug.Log("[Upgrade] Upuszczony obiekt nie jest przedmiotem.");
			return;
		}

		// Place the item visually as a child of this slot
		draggable.parentAfterDrag = this.transform;
		draggable.transform.SetParent(this.transform, false);
		placedItem = draggable;

		Debug.Log($"[Upgrade] Umieszczono przedmiot: {draggable.itemData.itemName}");

		// Notify manager to update UI
		if (manager != null) manager.OnItemPlaced(draggable);

		// Refresh mirrors so the mirror for this original is hidden
		if (UpgradeManager.Instance != null) UpgradeManager.Instance.RefreshMirrors();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		// Optional: visual highlight
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		// Optional: remove highlight
	}
}
