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

		// Determine the original item (do NOT reparent original into drop zone)
		DraggableItem visual = draggable;
		DraggableItem original = (draggable.isMirror && draggable.originalSource != null) ? draggable.originalSource : draggable;

		// Here you can filter item types if needed (e.g., only equipment)
		// For now accept only items with EquipmentItemData
		if (draggable.itemData == null)
		{
			Debug.Log("[Upgrade] Upuszczony obiekt nie jest przedmiotem.");
			return;
		}

		// Place the item visually as a child of this slot but keep the original in inventory
		placedItem = original;

		if (visual != null && visual.isMirror)
		{
			visual.parentAfterDrag = this.transform;
			visual.transform.SetParent(this.transform, false);
		}
		else
		{
			// No mirror was dragged; try to get existing mirror for this original
			GameObject mirror = UpgradeManager.Instance != null ? UpgradeManager.Instance.GetMirrorForOriginal(original) : null;
			if (mirror != null)
			{
				DraggableItem mirrorItem = mirror.GetComponent<DraggableItem>();
				if (mirrorItem != null) mirrorItem.parentAfterDrag = this.transform;
				mirror.transform.SetParent(this.transform, false);
			}
			else
			{
				// create a temporary mirror for visual feedback
				if (InventoryManager.Instance != null && InventoryManager.Instance.draggableItemPrefab != null && UpgradeManager.Instance != null)
				{
					GameObject tmp = Instantiate(InventoryManager.Instance.draggableItemPrefab, this.transform);
					DraggableItem dl = tmp.GetComponent<DraggableItem>();
					if (dl != null)
					{
						dl.Setup(original.itemData);
						dl.isMirror = true;
						dl.originalSource = original;
						dl.parentAfterDrag = this.transform;
						dl.upgradeLevel = original.upgradeLevel;
						dl.upgradePoints = (original.upgradePoints != null) ? new System.Collections.Generic.List<int>(original.upgradePoints) : new System.Collections.Generic.List<int>(new int[DraggableItem.UPGRADE_STAT_COUNT]);
						UpgradeManager.Instance.RegisterTemporaryMirror(original, tmp);
					}
				}
			}
		}

		Debug.Log($"[Upgrade] Umieszczono przedmiot: {draggable.itemData.itemName}");

		// Notify manager to update UI
		if (manager != null) manager.OnItemPlaced(original);

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
