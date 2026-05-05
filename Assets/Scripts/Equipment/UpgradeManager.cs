using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
	[Header("UI refs")]
	public UpgradeDropZone dropZone;
	public TextMeshProUGUI textChance;
	public TextMeshProUGUI textCost;
	public TextMeshProUGUI goldTxt;        // Nowy: Gold counter
	public TMP_Dropdown statSelectDropdown;
	public Button btnDoUpgrade;

	[Header("Inventory sync")]
	public Transform sourceInventoryGrid; // assign EquipmentWindow/BottomArea_Inventory/Grid_Inventory
	public Transform upgradeInventoryGrid; // assign Upgrade_Window/Equipment/Grid_Inventory

	public static UpgradeManager Instance;

	// mapping original draggable -> mirror GameObject in upgrade grid
	private System.Collections.Generic.List<System.Tuple<DraggableItem, GameObject>> mirrorMappings = new System.Collections.Generic.List<System.Tuple<DraggableItem, GameObject>>();
	private Transform runtimeSourceGrid;

	private DraggableItem currentItem;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		if (dropZone != null) dropZone.manager = this;
		if (btnDoUpgrade != null) btnDoUpgrade.onClick.AddListener(TryUpgrade);
		PopulateStatDropdown();
		if (statSelectDropdown != null)
			statSelectDropdown.onValueChanged.AddListener(_ => UpdateUI());
	}

	public void OpenWindow()
	{
		gameObject.SetActive(true);
		SyncInventoryToUpgradeGrid();
		RefreshMirrors();
		UpdateGoldUI();
		UpdateUI();
	}

	private void OnEnable()
	{
		currentItem = null;
		SyncInventoryToUpgradeGrid();
		UpdateGoldUI();
		UpdateUI();
	}

	private void OnDisable()
	{
		// destroy mirrors when closing
		ClearMirrors();
		currentItem = null;
		if (dropZone != null) dropZone.placedItem = null;
	}

	private void SyncInventoryToUpgradeGrid()
	{
		// create mirror visuals instead of moving originals
		ClearMirrors();
		if (InventoryManager.Instance == null || upgradeInventoryGrid == null) return;

		runtimeSourceGrid = ResolveSourceInventoryGrid();
		Transform invGrid = runtimeSourceGrid;
		if (invGrid == null)
		{
			Debug.LogWarning("[Upgrade] Nie udało się ustalić źródłowego Grid_Inventory dla mirrorów.");
			return;
		}

		ConsolidateAllItemsToSourceGrid(invGrid);

		int slotCount = Mathf.Min(invGrid.childCount, upgradeInventoryGrid.childCount);
		for (int i = 0; i < slotCount; i++)
		{
			Transform upSlot = upgradeInventoryGrid.GetChild(i);
			Transform sourceSlot = invGrid.GetChild(i);
			if (sourceSlot.childCount <= 0) continue;
			DraggableItem original = sourceSlot.GetChild(0).GetComponent<DraggableItem>();
			if (original == null || original.isMirror) continue;

			GameObject mirror = Instantiate(InventoryManager.Instance.draggableItemPrefab, upSlot);
			RectTransform rect = mirror.GetComponent<RectTransform>();
			rect.localScale = Vector3.one;
			rect.anchorMin = new Vector2(0, 0);
			rect.anchorMax = new Vector2(1, 1);
			rect.offsetMin = Vector2.zero;
			rect.offsetMax = Vector2.zero;
			rect.anchoredPosition = Vector2.zero;

			DraggableItem mirrorLogic = mirror.GetComponent<DraggableItem>();
			if (mirrorLogic != null)
			{
				mirrorLogic.Setup(original.itemData);
				mirrorLogic.isMirror = true;
				mirrorLogic.originalSource = original;
				mirrorLogic.upgradeLevel = original.upgradeLevel;
				mirrorLogic.upgradePoints = (original.upgradePoints != null) ? new System.Collections.Generic.List<int>(original.upgradePoints) : new System.Collections.Generic.List<int>(new int[DraggableItem.UPGRADE_STAT_COUNT]);
			}

			mirrorMappings.Add(new System.Tuple<DraggableItem, GameObject>(original, mirror));
		}

		Debug.Log($"[Upgrade] Utworzono {mirrorMappings.Count} mirrorów itemów w oknie ulepszania.");
	}

	private Transform ResolveSourceInventoryGrid()
	{
		Debug.Log("[Upgrade] ResolveSourceInventoryGrid() START");
		
		if (sourceInventoryGrid != null) 
		{
			Debug.Log($"[Upgrade] Zwracam podpięty sourceInventoryGrid: {sourceInventoryGrid.name}");
			return sourceInventoryGrid;
		}

		// preferred explicit source
		if (InventoryManager.Instance != null && InventoryManager.Instance.gridInventory != null)
		{
			Transform preferred = InventoryManager.Instance.gridInventory;
			Debug.Log($"[Upgrade] Zwracam gridInventory z InventoryManager: {preferred.name}");
			return preferred;
		}

		// fallback: pick grid named Grid_Inventory with the highest item count (excluding upgrade grid)
		Transform best = null;
		int bestCount = -1;
		var allSlots = FindObjectsByType<ItemSlot>(FindObjectsInactive.Include, FindObjectsSortMode.None);
		Debug.Log($"[Upgrade] Znaleziono {allSlots.Length} ItemSlotów");
		
		Dictionary<Transform, int> gridCounts = new Dictionary<Transform, int>();
		foreach (var slot in allSlots)
		{
			if (slot == null || slot.isEquippedSlot) continue;
			Transform grid = slot.transform.parent;
			if (grid == null) continue;
			if (upgradeInventoryGrid != null && (grid == upgradeInventoryGrid || grid.IsChildOf(upgradeInventoryGrid))) continue;
			if (grid.name != "Grid_Inventory") continue;
			if (!gridCounts.ContainsKey(grid)) gridCounts[grid] = 0;
			if (slot.transform.childCount > 0) gridCounts[grid]++;
		}

		Debug.Log($"[Upgrade] Znaleziono {gridCounts.Count} Grid_Inventory zasobów");
		
		foreach (var kv in gridCounts)
		{
			Debug.Log($"[Upgrade]   - Grid: {kv.Key.name}, ItemCount: {kv.Value}");
			if (kv.Value > bestCount)
			{
				best = kv.Key;
				bestCount = kv.Value;
			}
		}

		if (best != null)
			Debug.Log($"[Upgrade] Zwracam best Grid_Inventory: {best.name} z {bestCount} itemami");
		else
			Debug.LogError("[Upgrade] BŁĄD: Nie znaleziono żadnego Grid_Inventory!");

		return best;
	}

	private void ConsolidateAllItemsToSourceGrid(Transform sourceGrid)
	{
		if (sourceGrid == null) return;

		// Gather all non-equipped, non-mirror items from any other inventory grid.
		var allSlots = FindObjectsByType<ItemSlot>(FindObjectsInactive.Include, FindObjectsSortMode.None);
		List<DraggableItem> itemsToMove = new List<DraggableItem>();
		foreach (var slot in allSlots)
		{
			if (slot == null || slot.isEquippedSlot) continue;
			Transform grid = slot.transform.parent;
			if (grid == null || grid == sourceGrid) continue;
			if (upgradeInventoryGrid != null && (grid == upgradeInventoryGrid || grid.IsChildOf(upgradeInventoryGrid))) continue;
			if (slot.transform.childCount <= 0) continue;
			DraggableItem it = slot.transform.GetChild(0).GetComponent<DraggableItem>();
			if (it == null || it.isMirror) continue;
			itemsToMove.Add(it);
		}

		if (itemsToMove.Count == 0) return;

		// Build lookup by slot name in source grid.
		Dictionary<string, Transform> sourceByName = new Dictionary<string, Transform>();
		for (int i = 0; i < sourceGrid.childCount; i++)
		{
			Transform s = sourceGrid.GetChild(i);
			if (!sourceByName.ContainsKey(s.name)) sourceByName.Add(s.name, s);
		}

		foreach (var it in itemsToMove)
		{
			Transform fromSlot = it.transform.parent;
			Transform targetSlot = null;
			if (fromSlot != null && sourceByName.ContainsKey(fromSlot.name))
			{
				targetSlot = sourceByName[fromSlot.name];
			}
			if (targetSlot == null || targetSlot.childCount > 0)
			{
				for (int i = 0; i < sourceGrid.childCount; i++)
				{
					Transform empty = sourceGrid.GetChild(i);
					if (empty.childCount == 0) { targetSlot = empty; break; }
				}
			}
			if (targetSlot != null)
			{
				it.parentAfterDrag = targetSlot;
				it.transform.SetParent(targetSlot, false);
			}
		}
	}

	private int CountItemsInGrid(Transform grid)
	{
		if (grid == null) return 0;
		int count = 0;
		for (int i = 0; i < grid.childCount; i++)
		{
			if (grid.GetChild(i).childCount > 0) count++;
		}
		return count;
	}

	private void ClearMirrors()
	{
		for (int i = mirrorMappings.Count - 1; i >= 0; i--)
		{
			var pair = mirrorMappings[i];
			if (pair == null) continue;
			GameObject mirrorObj = pair.Item2;
			if (mirrorObj != null) Destroy(mirrorObj);
		}
		mirrorMappings.Clear();
	}

	// called externally (e.g., ItemSlot.OnDrop) to refresh item visibility
	public void RefreshMirrors()
	{
		for (int i = mirrorMappings.Count - 1; i >= 0; i--)
		{
			var pair = mirrorMappings[i];
			DraggableItem original = pair.Item1;
			GameObject mirrorObj = pair.Item2;
			if (original == null || mirrorObj == null)
			{
				if (mirrorObj != null) Destroy(mirrorObj);
				mirrorMappings.RemoveAt(i);
				continue;
			}
			// if original was moved out of its slot or removed, hide/destroy mirror
			if (original.transform.parent == null || original.transform.parent.IsChildOf(upgradeInventoryGrid))
			{
				Destroy(mirrorObj);
				mirrorMappings.RemoveAt(i);
				continue;
			}
			// sync upgrade level/state
			DraggableItem mirrorLogic = mirrorObj.GetComponent<DraggableItem>();
			if (mirrorLogic != null)
			{
				mirrorLogic.upgradeLevel = original.upgradeLevel;
				mirrorLogic.upgradePoints = (original.upgradePoints != null) ? new System.Collections.Generic.List<int>(original.upgradePoints) : new System.Collections.Generic.List<int>(new int[DraggableItem.UPGRADE_STAT_COUNT]);
			}
		}
	}

	public GameObject GetMirrorForOriginal(DraggableItem original)
	{
		if (original == null) return null;
		foreach (var pair in mirrorMappings)
		{
			if (pair != null && pair.Item1 == original) return pair.Item2;
		}
		return null;
	}

	public void RegisterTemporaryMirror(DraggableItem original, GameObject mirror)
	{
		if (original == null || mirror == null) return;
		mirrorMappings.Add(new System.Tuple<DraggableItem, GameObject>(original, mirror));
	}

	private void PopulateStatDropdown()
	{
		if (statSelectDropdown == null) return;

		statSelectDropdown.ClearOptions();
		List<string> options = new List<string>() {
			"Brak",
			"Punkty Życia",
			"Siła",
			"Zręczność",
			"Kondycja",
			"Mana",
			"Pancerz Fizyczny",
			"Odporność Magiczna",
			"Obrażenia Broni",
			"Szansa Krytyczna",
			"Szansa Uniku",
			"Mnożnik Obrażeń",
			"Mnożnik Trafienia"
		};
		statSelectDropdown.AddOptions(options);
		statSelectDropdown.value = 0;
	}

	public void OnItemPlaced(DraggableItem item)
	{
		if (item != null && item.isMirror && item.originalSource != null) currentItem = item.originalSource;
		else currentItem = item;
		UpdateUI();
	}

	public void ClearPlacedItem()
	{
		currentItem = null;
		UpdateUI();
	}

	private bool IsItemMirrorOnDropZone(DraggableItem original)
	{
		if (dropZone == null || original == null) return false;
		for (int i = 0; i < dropZone.transform.childCount; i++)
		{
			var d = dropZone.transform.GetChild(i).GetComponent<DraggableItem>();
			if (d != null && d.isMirror && d.originalSource == original)
				return true;
		}
		return false;
	}

	private bool CanAttemptUpgrade(out string blockReason)
	{
		blockReason = null;
		if (GameManager.Instance == null)
		{
			blockReason = "brak systemu ekonomii";
			return false;
		}
		if (dropZone == null || currentItem == null)
		{
			blockReason = "brak przedmiotu na kowadle";
			return false;
		}
		if (dropZone.placedItem != currentItem || !IsItemMirrorOnDropZone(currentItem))
		{
			blockReason = "przedmiot musi być na kowadle";
			return false;
		}
		int statIndex = statSelectDropdown != null ? statSelectDropdown.value : 0;
		if (statIndex <= 0 || statIndex >= DraggableItem.UPGRADE_STAT_COUNT)
		{
			blockReason = "wybierz statystykę do ulepszenia";
			return false;
		}
		currentItem.EnsureUpgradePointsList();
		int tier = DraggableItem.SumAllocatedUpgradePoints(currentItem);
		int cost = 100 + tier * 200;
		if (GameManager.Instance.globalGold < cost)
		{
			blockReason = "niewystarczające złoto";
			return false;
		}
		return true;
	}

	public void UpdateUI()
	{
		currentItem?.EnsureUpgradePointsList();
		if (currentItem == null || dropZone == null || dropZone.placedItem != currentItem || !IsItemMirrorOnDropZone(currentItem))
		{
			if (currentItem != null && (dropZone == null || dropZone.placedItem != currentItem || !IsItemMirrorOnDropZone(currentItem)))
				currentItem = null;

			if (textChance != null) textChance.text = "-";
			if (textCost != null) textCost.text = "-";
			if (btnDoUpgrade != null) btnDoUpgrade.interactable = false;
			return;
		}

		int statIndex = statSelectDropdown != null ? statSelectDropdown.value : 0;
		int tier = DraggableItem.SumAllocatedUpgradePoints(currentItem);
		int chance = Mathf.Clamp(100 - tier * 7, 5, 100);
		int cost = 100 + tier * 200;
		Debug.Log($"[Upgrade] Item: {currentItem.itemData?.itemName} | ulepszeń(stat)={tier} | szansa={chance}%");

		if (textChance != null) textChance.text = $"Szansa: {chance}%";
		if (textCost != null) textCost.text = $"Koszt: {cost} zł";
		bool canUpgrade = GameManager.Instance != null
		                  && statIndex > 0 && statIndex < DraggableItem.UPGRADE_STAT_COUNT
		                  && GameManager.Instance.globalGold >= cost;
		if (btnDoUpgrade != null) btnDoUpgrade.interactable = canUpgrade;
	}

	public void TryUpgrade()
	{
		if (currentItem != null && currentItem.isMirror && currentItem.originalSource != null)
			currentItem = currentItem.originalSource;

		if (!CanAttemptUpgrade(out string reason))
		{
			Debug.Log($"[Upgrade] Próba odrzucona: {reason}.");
			return;
		}

		currentItem.EnsureUpgradePointsList();
		int tier = DraggableItem.SumAllocatedUpgradePoints(currentItem);
		int chance = Mathf.Clamp(100 - tier * 7, 5, 100);
		int cost = 100 + tier * 200;

		if (GameManager.Instance == null || !GameManager.Instance.SpendGold(cost))
		{
			Debug.Log("[Upgrade] Nie można pobrać kosztu (brak złota).");
			UpdateUI();
			return;
		}

		UpdateGoldUI();

		int roll = Random.Range(1, 101);
		bool success = roll <= chance;
		Debug.Log($"[Upgrade] Roll={roll} vs Chance={chance}% for {currentItem.itemData?.itemName}");

		if (success)
		{
			int statIndex = statSelectDropdown != null ? statSelectDropdown.value : 0;
			if (statIndex <= 0 || statIndex >= DraggableItem.UPGRADE_STAT_COUNT)
			{
				if (GameManager.Instance != null) GameManager.Instance.AddGold(cost);
				UpdateGoldUI();
				Debug.LogError("[Upgrade] Niespójny stan — zwracam złoto.");
				return;
			}
			currentItem.upgradePoints[statIndex]++;
			currentItem.SyncUpgradeLevelFromAllocations();
			Debug.Log($"[Upgrade] Sukces! Stat index {statIndex} +1. Łącznie ulepszeń: {currentItem.upgradeLevel}");
		}
		else
		{
			// Failure can burn the item permanently.
			int burnChance = Mathf.Clamp(tier * 5, 0, 85);
			bool burned = Random.Range(1, 101) <= burnChance;
			if (burned)
			{
				Debug.Log($"[Upgrade] Porażka i spalenie przedmiotu! Szansa spalenia: {burnChance}%.");
				DestroyCurrentItemPermanently();
				return;
			}
			Debug.Log("[Upgrade] Nieudane ulepszenie, ale przedmiot ocalał.");
		}

		// Update UI and persist inventory
		UpdateUI();
		if (EquipmentStatsCalculator.Instance != null) EquipmentStatsCalculator.Instance.RecalculateAllEquipmentStats();
		RefreshMirrors();
		if (InventorySaveSystem.Instance != null) InventorySaveSystem.Instance.SaveInventory();
	}

	private void DestroyCurrentItemPermanently()
	{
		if (currentItem == null) return;
		if (dropZone != null && dropZone.placedItem == currentItem) dropZone.placedItem = null;
		if (ItemTooltip.Instance != null) ItemTooltip.Instance.HideTooltip();

		GameObject doomed = currentItem.gameObject;
		currentItem = null;
		Destroy(doomed);

		UpdateUI();
		if (EquipmentStatsCalculator.Instance != null) EquipmentStatsCalculator.Instance.RecalculateAllEquipmentStats();
		RefreshMirrors();
		if (InventorySaveSystem.Instance != null) InventorySaveSystem.Instance.SaveInventory();
	}

	public void UpdateGoldUI()
	{
		if (goldTxt != null && GameManager.Instance != null)
			goldTxt.text = "ZŁOTO: " + GameManager.Instance.globalGold;
	}
}
