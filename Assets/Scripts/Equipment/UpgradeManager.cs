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

	// mapping: original slot in source grid -> stored items (for restoring)
	private Dictionary<Transform, List<DraggableItem>> originalSlotContents = new Dictionary<Transform, List<DraggableItem>>();
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
	}

	public void OpenWindow()
	{
		gameObject.SetActive(true);
		SyncInventoryToUpgradeGrid();
		RefreshMirrors();
		UpdateGoldUI();
	}

	private void OnEnable()
	{
		SyncInventoryToUpgradeGrid();
		UpdateGoldUI();
	}

	private void OnDisable()
	{
		ReturnInventoryToOriginalSlots();
	}

	private void SyncInventoryToUpgradeGrid()
	{
		// Nie tworzymy mirrorów - PRZESUNIEMY rzeczywiste przedmioty z Equipment_Window do Upgrade_Window
		originalSlotContents.Clear();
		
		Debug.Log($"[Upgrade] SyncInventoryToUpgradeGrid START - InventoryManager: {(InventoryManager.Instance != null)}, upgradeInventoryGrid: {(upgradeInventoryGrid != null)}");
		
		// InventoryManager może być null na starcie - to normalne
		if (upgradeInventoryGrid == null) { Debug.LogError("[Upgrade] BŁĄD: upgradeInventoryGrid jest NULL! Podepnij go w Inspectorze UpgradeManager!"); return; }

		runtimeSourceGrid = ResolveSourceInventoryGrid();
		Debug.Log($"[Upgrade] runtimeSourceGrid resolved: {(runtimeSourceGrid != null ? runtimeSourceGrid.name : "NULL")}");
		
		if (runtimeSourceGrid == null)
		{
			Debug.LogError("[Upgrade] BŁĄD: Nie znaleziono sourceGrid! Sprawdź Grid_Inventory w Equipment_Window");
			return;
		}

		ConsolidateAllItemsToSourceGrid(runtimeSourceGrid);

		// Zachowaj oryginalne pozycje przedmiotów
		for (int i = 0; i < runtimeSourceGrid.childCount; i++)
		{
			Transform sourceSlot = runtimeSourceGrid.GetChild(i);
			List<DraggableItem> slotItems = new List<DraggableItem>();
			
			for (int j = sourceSlot.childCount - 1; j >= 0; j--)
			{
				DraggableItem item = sourceSlot.GetChild(j).GetComponent<DraggableItem>();
				if (item != null && !item.isMirror)
					slotItems.Add(item);
			}
			
			if (slotItems.Count > 0)
				originalSlotContents[sourceSlot] = slotItems;
		}

		// Przesuń przedmioty do Upgrade_Window
		int totalMoved = 0;
		Debug.Log($"[Upgrade] Przesuwam przedmioty: sourceGrid ma {runtimeSourceGrid.childCount} slotów, upgradeGrid ma {upgradeInventoryGrid.childCount} slotów");
		
		for (int i = 0; i < Mathf.Min(runtimeSourceGrid.childCount, upgradeInventoryGrid.childCount); i++)
		{
			Transform sourceSlot = runtimeSourceGrid.GetChild(i);
			Transform upgradeSlot = upgradeInventoryGrid.GetChild(i);
			
			// Przesuń wszystkie przedmioty z sourceSlot do upgradeSlot
			while (sourceSlot.childCount > 0)
			{
				Transform itemTransform = sourceSlot.GetChild(0);
				DraggableItem item = itemTransform.GetComponent<DraggableItem>();
				
				if (item != null && !item.isMirror)
				{
					itemTransform.SetParent(upgradeSlot, false);
					RectTransform rect = itemTransform.GetComponent<RectTransform>();
					if (rect != null)
					{
						rect.localScale = Vector3.one;
						rect.anchorMin = new Vector2(0, 0);
						rect.anchorMax = new Vector2(1, 1);
						rect.offsetMin = Vector2.zero;
						rect.offsetMax = Vector2.zero;
						rect.anchoredPosition = Vector2.zero;
					}
					totalMoved++;
					Debug.Log($"[Upgrade] Przesunięty item: {item.itemData?.itemName} do slotu {i}");
				}
				else
				{
					break;
				}
			}
		}

		Debug.Log($"[Upgrade] SKOŃCZYŁEM - Przesunięto {totalMoved} przedmiotów do Upgrade_Window");
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

	private void ReturnInventoryToOriginalSlots()
	{
		// Przywróć przedmioty do originalnych slotów w sourceGrid
		if (runtimeSourceGrid == null) return;

		foreach (var kvp in originalSlotContents)
		{
			Transform originalSlot = kvp.Key;
			List<DraggableItem> items = kvp.Value;
			
			foreach (var item in items)
			{
				if (item != null)
				{
					item.transform.SetParent(originalSlot, false);
					RectTransform rect = item.GetComponent<RectTransform>();
					if (rect != null)
					{
						rect.localScale = Vector3.one;
						rect.anchorMin = new Vector2(0, 0);
						rect.anchorMax = new Vector2(1, 1);
						rect.offsetMin = Vector2.zero;
						rect.offsetMax = Vector2.zero;
						rect.anchoredPosition = Vector2.zero;
					}
				}
			}
		}
		
		originalSlotContents.Clear();
		Debug.Log("[Upgrade] Przywrócono przedmioty do Equipment_Window.");
	}

	// Nie używamy już mirrorów - usunięto ClearMirrors()

	// called externally (e.g., ItemSlot.OnDrop) to refresh item visibility
	public void RefreshMirrors()
	{
		// Nowy system: przedmioty są przesuniętymi rzeczywistymi obiektami, nie mirrorami
		// Nic nie trzeba robić - przedmioty są zawsze w poprawnym miejscu
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

	public void UpdateUI()
	{
		if (currentItem == null)
		{
			if (textChance != null) textChance.text = "-";
			if (textCost != null) textCost.text = "-";
			if (btnDoUpgrade != null) btnDoUpgrade.interactable = false;
			return;
		}

		// Example calculation (replace with your real formula later)
		int level = currentItem.upgradeLevel;
		int chance = Mathf.Clamp(100 - level * 7, 5, 100);
		int cost = 100 + level * 200;
		Debug.Log($"[Upgrade] Item: {currentItem.itemData?.itemName} | itemLevel={level} | chance={chance}%");

		if (textChance != null) textChance.text = $"Szansa: {chance}%";
		if (textCost != null) textCost.text = $"Koszt: {cost} zł";
		if (btnDoUpgrade != null) btnDoUpgrade.interactable = true;
	}

	public void TryUpgrade()
	{
		if (currentItem == null) return;
		if (currentItem.isMirror && currentItem.originalSource != null) currentItem = currentItem.originalSource;

		int level = currentItem.upgradeLevel;
		int chance = Mathf.Clamp(100 - level * 7, 5, 100);

		int roll = Random.Range(1, 101);
		bool success = roll <= chance;
		Debug.Log($"[Upgrade] Roll={roll} vs Chance={chance}% for {currentItem.itemData?.itemName}");

		if (success)
		{
			// Apply upgrade to selected stat
			int statIndex = statSelectDropdown != null ? statSelectDropdown.value : 0;
			if (statIndex > 0 && statIndex < DraggableItem.UPGRADE_STAT_COUNT)
			{
				// ensure list length
				if (currentItem.upgradePoints == null) currentItem.upgradePoints = new System.Collections.Generic.List<int>(new int[DraggableItem.UPGRADE_STAT_COUNT]);
				if (currentItem.upgradePoints.Count < DraggableItem.UPGRADE_STAT_COUNT)
				{
					int need = DraggableItem.UPGRADE_STAT_COUNT - currentItem.upgradePoints.Count;
					for (int i = 0; i < need; i++) currentItem.upgradePoints.Add(0);
				}
				currentItem.upgradePoints[statIndex]++;
				currentItem.upgradeLevel++;
				Debug.Log($"[Upgrade] Sukces! Stat index {statIndex} +1. Poziom: {currentItem.upgradeLevel}");
			}
			else
			{
				// fallback: just increment total level
				currentItem.upgradeLevel++;
				Debug.Log($"[Upgrade] Sukces! Nowy poziom: {currentItem.upgradeLevel}");
			}
		}
		else
		{
			// Failure can burn the item permanently.
			int burnChance = Mathf.Clamp(level * 5, 0, 85);
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
