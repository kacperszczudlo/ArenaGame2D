using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class QuestManager : MonoBehaviour
{
	private const string QuestItemsResourcesFolder = "QuestItems";
	private const string SnowBiomeHint = "w Śnieżnej Twierdzy";
	private const string AshBiomeHint = "na Popielnych Pustkowiach";
	private const string LavaBiomeHint = "w Krainie Lawy";
	private const string ForestBiomeHint = "w Lesie";
	private const string BeachBiomeHint = "na Piaszczystej Plaży";

	private const char SnowBiomeCode = '1';
	private const char AshBiomeCode = '2';
	private const char LavaBiomeCode = '3';
	private const char ForestBiomeCode = '4';
	private const char BeachBiomeCode = '5';

	private const string BiomeGridLayoutDefault =
		"0000111000000000\n" +
		"0000111111111110\n" +
		"0000111111111110\n" +
		"0000111111110000\n" +
		"0000002211110000\n" +
		"0002222201100000\n" +
		"0002222201100000\n" +
		"0002222200000000\n" +
		"0002222200000000\n" +
		"0000033330000000\n" +
		"0000033330000000\n" +
		"0000444440000000\n" +
		"0000444444000000\n" +
		"5555544444400000\n" +
		"5555544444400000\n" +
		"5555544444400000\n" +
		"5555544444400000\n" +
		"5555500000000000";

	public static QuestManager Instance { get; private set; }

	[Header("Tilemapy dozwolone")]
	public Tilemap groundMap;
	public Tilemap stairsMap;
	public Tilemap bridgeMap;

	[Header("Tilemapy blokujące")]
	public Tilemap[] blockingMaps;

	[Header("Prefab przedmiotu questowego")]
	public GameObject questItemPrefab;

	[HideInInspector] public bool isQuestActive;
	[HideInInspector] public bool hasCollectedItem;
	[HideInInspector] public string targetItemName;
	[HideInInspector] public Sprite targetItemSprite;
	[HideInInspector] public string biomeHint;
	[HideInInspector] public int calculatedGoldReward;

	public Vector3Int questSpawnCell { get; private set; }
	public Vector3 questSpawnWorldPosition { get; private set; }

	private GameObject spawnedItemInstance;
	private List<string> cachedBiomeGridRows;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public bool GenerateNewQuest()
	{
		if (isQuestActive)
		{
			Debug.LogWarning("[QUEST] Quest jest już aktywny.");
			return false;
		}

		Sprite[] allItems = Resources.LoadAll<Sprite>(QuestItemsResourcesFolder);
		if (allItems == null || allItems.Length == 0)
		{
			Debug.LogError($"[QUEST] Nie znaleziono ikon w Resources/{QuestItemsResourcesFolder}.");
			return false;
		}

		Vector3Int foundSpawnCell;
		if (!TryFindValidSpawnLocation(out foundSpawnCell))
		{
			return false;
		}

		questSpawnCell = foundSpawnCell;

		targetItemSprite = allItems[UnityEngine.Random.Range(0, allItems.Length)];
		targetItemName = targetItemSprite.name;
		questSpawnWorldPosition = GetWorldPosition(questSpawnCell);
		biomeHint = ResolveBiomeHint(questSpawnCell, questSpawnWorldPosition, out string biomeSource);
		calculatedGoldReward = CalculateQuestReward();
		isQuestActive = true;
		hasCollectedItem = false;

		SpawnQuestItemOnMap();

		Debug.Log($"[QUEST] Wylosowano zadanie. Przynieś: {targetItemName}. Szukaj {biomeHint}. Nagroda: {calculatedGoldReward}g. Spawn: {questSpawnWorldPosition}, Cell: {questSpawnCell}, Źródło biomu: {biomeSource}");
		return true;
	}

	public bool TryCompleteQuest(string deliveredItemName)
	{
		if (!isQuestActive)
		{
			Debug.LogWarning("[QUEST] Nie ma aktywnego questa do oddania.");
			return false;
		}

		if (!hasCollectedItem)
		{
			Debug.LogWarning("[QUEST] Nie zebrano jeszcze przedmiotu questowego.");
			return false;
		}

		if (string.IsNullOrWhiteSpace(deliveredItemName))
		{
			return false;
		}

		if (!string.Equals(deliveredItemName, targetItemName, StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}

		if (GameManager.Instance != null)
		{
			GameManager.Instance.AddGold(calculatedGoldReward);
		}
		else
		{
			Debug.LogWarning("[QUEST] Brak GameManager.Instance, nagroda nie została dodana.");
		}

		Debug.Log($"[QUEST] Quest oddany. Dodano {calculatedGoldReward}g za {targetItemName}.");
		ClearQuestState();
		return true;
	}

	public bool ItemCollectedByPlayer(string collectedItemName)
	{
		if (!isQuestActive || hasCollectedItem)
		{
			return false;
		}

		if (string.IsNullOrWhiteSpace(collectedItemName))
		{
			return false;
		}

		if (!string.Equals(collectedItemName, targetItemName, StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}

		hasCollectedItem = true;

		if (spawnedItemInstance != null)
		{
			Destroy(spawnedItemInstance);
			spawnedItemInstance = null;
		}

		Debug.Log($"[QUEST] Zebrano przedmiot: {targetItemName}. Wróć do NPC po nagrodę.");
		return true;
	}

	public void ClearQuestState()
	{
		if (spawnedItemInstance != null)
		{
			Destroy(spawnedItemInstance);
			spawnedItemInstance = null;
		}

		isQuestActive = false;
		hasCollectedItem = false;
		targetItemName = string.Empty;
		targetItemSprite = null;
		biomeHint = string.Empty;
		calculatedGoldReward = 0;
		questSpawnCell = default;
		questSpawnWorldPosition = Vector3.zero;
	}

	private int CalculateQuestReward()
	{
		EnemyData rewardSource = GameManager.Instance != null ? GameManager.Instance.currentEnemyToFight : null;

		if (rewardSource == null)
		{
			return 10;
		}

		return Mathf.RoundToInt(rewardSource.goldReward / 2f);
	}

	private bool TryFindValidSpawnLocation(out Vector3Int spawnCell)
	{
		HashSet<Vector3Int> traversableCells = new HashSet<Vector3Int>();
		CollectTraversableCellsFromMap(groundMap, traversableCells);
		CollectTraversableCellsFromMap(stairsMap, traversableCells);
		CollectTraversableCellsFromMap(bridgeMap, traversableCells);

		HashSet<Vector3Int> validCells = new HashSet<Vector3Int>();
		foreach (Vector3Int cell in traversableCells)
		{
			if (!IsBlocked(cell))
			{
				validCells.Add(cell);
			}
		}

		if (validCells.Count == 0)
		{
			spawnCell = default;
			Debug.LogWarning($"[QUEST] Nie udało się znaleźć poprawnego miejsca na spawn przedmiotu. Traversable: {traversableCells.Count}, po filtrze blockingMaps: {validCells.Count}. Sprawdź czy mapy biome (np. Snow) nie są omyłkowo traktowane jako blokujące.");
			return false;
		}

		int selectedIndex = UnityEngine.Random.Range(0, validCells.Count);
		int currentIndex = 0;

		foreach (Vector3Int cell in validCells)
		{
			if (currentIndex == selectedIndex)
			{
				spawnCell = cell;
				return true;
			}

			currentIndex++;
		}

		spawnCell = default;
		return false;
	}

	private void CollectTraversableCellsFromMap(Tilemap tilemap, HashSet<Vector3Int> traversableCells)
	{
		if (tilemap == null)
		{
			return;
		}

		BoundsInt bounds = tilemap.cellBounds;
		foreach (Vector3Int cell in bounds.allPositionsWithin)
		{
			if (!tilemap.HasTile(cell))
			{
				continue;
			}

			traversableCells.Add(cell);
		}
	}

	private bool IsBlocked(Vector3Int cell)
	{
		if (blockingMaps == null)
		{
			return false;
		}

		foreach (Tilemap blockingMap in blockingMaps)
		{
			if (blockingMap == null)
			{
				continue;
			}

			if (blockingMap != null && blockingMap.HasTile(cell))
			{
				return true;
			}
		}

		return false;
	}

	private Vector3 GetWorldPosition(Vector3Int cell)
	{
		Tilemap referenceMap = null;
		if (groundMap != null && groundMap.HasTile(cell))
		{
			referenceMap = groundMap;
		}
		else if (stairsMap != null && stairsMap.HasTile(cell))
		{
			referenceMap = stairsMap;
		}
		else if (bridgeMap != null && bridgeMap.HasTile(cell))
		{
			referenceMap = bridgeMap;
		}
		else
		{
			referenceMap = groundMap != null ? groundMap : stairsMap != null ? stairsMap : bridgeMap;
		}

		if (referenceMap != null)
		{
			return referenceMap.GetCellCenterWorld(cell);
		}

		return Vector3.zero;
	}

	private string ResolveBiomeHint(Vector3Int cell, Vector3 worldPosition, out string source)
	{
		if (TryResolveBiomeByGrid(cell, out string gridSource, out string gridBiomeHint))
		{
			source = gridSource;
			return gridBiomeHint;
		}

		if (TryResolveBiomeByTileName(cell, out string tileNameSource, out string tileNameBiomeHint))
		{
			source = tileNameSource;
			return tileNameBiomeHint;
		}

		source = "fallback-default:ash";
		return AshBiomeHint;
	}

	private bool TryResolveBiomeByGrid(Vector3Int cell, out string source, out string hint)
	{
		source = string.Empty;
		hint = string.Empty;

		if (!TryGetBiomeGridRows(out List<string> rows))
		{
			return false;
		}

		if (!TryGetTraversableAreaBounds(out int minX, out int maxX, out int minY, out int maxY))
		{
			return false;
		}

		int width = maxX - minX + 1;
		int height = maxY - minY + 1;
		if (width <= 0 || height <= 0)
		{
			return false;
		}

		int cols = rows[0].Length;
		int rowCount = rows.Count;

		float normalizedX = (cell.x - minX) / (float)width;
		float normalizedY = (cell.y - minY) / (float)height;

		int col = Mathf.Clamp(Mathf.FloorToInt(normalizedX * cols), 0, cols - 1);
		int rowFromBottom = Mathf.Clamp(Mathf.FloorToInt(normalizedY * rowCount), 0, rowCount - 1);
		int row = rowCount - 1 - rowFromBottom;

		char code = rows[row][col];
		if (code == SnowBiomeCode)
		{
			source = $"grid:{code}:snow(r{row},c{col})";
			hint = SnowBiomeHint;
			return true;
		}

		if (code == AshBiomeCode)
		{
			source = $"grid:{code}:ash(r{row},c{col})";
			hint = AshBiomeHint;
			return true;
		}

		if (code == LavaBiomeCode)
		{
			source = $"grid:{code}:lava(r{row},c{col})";
			hint = LavaBiomeHint;
			return true;
		}

		if (code == ForestBiomeCode)
		{
			source = $"grid:{code}:forest(r{row},c{col})";
			hint = ForestBiomeHint;
			return true;
		}

		if (code == BeachBiomeCode)
		{
			source = $"grid:{code}:beach(r{row},c{col})";
			hint = BeachBiomeHint;
			return true;
		}

		return false;
	}

	private bool TryGetBiomeGridRows(out List<string> rows)
	{
		if (cachedBiomeGridRows != null)
		{
			rows = cachedBiomeGridRows;
			return rows.Count > 0;
		}

		rows = new List<string>();
		if (string.IsNullOrWhiteSpace(BiomeGridLayoutDefault))
		{
			cachedBiomeGridRows = rows;
			return false;
		}

		string[] rawRows = BiomeGridLayoutDefault.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
		int expectedLength = -1;

		foreach (string raw in rawRows)
		{
			string line = raw.Trim();
			if (line.Length == 0)
			{
				continue;
			}

			if (expectedLength < 0)
			{
				expectedLength = line.Length;
			}
			else if (line.Length != expectedLength)
			{
				Debug.LogWarning("[QUEST] biomeGridLayout ma różne długości wierszy. Pomijam siatkę biomów.");
				cachedBiomeGridRows = new List<string>();
				rows = cachedBiomeGridRows;
				return false;
			}

			rows.Add(line);
		}

		cachedBiomeGridRows = rows;
		return rows.Count > 0;
	}

	private bool TryGetTraversableAreaBounds(out int minX, out int maxX, out int minY, out int maxY)
	{
		bool found = false;
		minX = int.MaxValue;
		maxX = int.MinValue;
		minY = int.MaxValue;
		maxY = int.MinValue;

		CollectBoundsFromMap(groundMap, ref found, ref minX, ref maxX, ref minY, ref maxY);
		CollectBoundsFromMap(stairsMap, ref found, ref minX, ref maxX, ref minY, ref maxY);
		CollectBoundsFromMap(bridgeMap, ref found, ref minX, ref maxX, ref minY, ref maxY);

		return found;
	}

	private void CollectBoundsFromMap(Tilemap tilemap, ref bool found, ref int minX, ref int maxX, ref int minY, ref int maxY)
	{
		if (tilemap == null)
		{
			return;
		}

		BoundsInt bounds = tilemap.cellBounds;
		foreach (Vector3Int cell in bounds.allPositionsWithin)
		{
			if (!tilemap.HasTile(cell))
			{
				continue;
			}

			found = true;
			if (cell.x < minX) minX = cell.x;
			if (cell.x > maxX) maxX = cell.x;
			if (cell.y < minY) minY = cell.y;
			if (cell.y > maxY) maxY = cell.y;
		}
	}

	private bool TryResolveBiomeByTileName(Vector3Int cell, out string source, out string hint)
	{
		TileBase tile = null;

		if (groundMap != null && groundMap.HasTile(cell))
		{
			tile = groundMap.GetTile(cell);
		}
		else if (stairsMap != null && stairsMap.HasTile(cell))
		{
			tile = stairsMap.GetTile(cell);
		}
		else if (bridgeMap != null && bridgeMap.HasTile(cell))
		{
			tile = bridgeMap.GetTile(cell);
		}

		if (tile == null)
		{
			source = string.Empty;
			hint = string.Empty;
			return false;
		}

		string tileName = tile.name != null ? tile.name.ToLowerInvariant() : string.Empty;

		if (tileName.Contains("snow") || tileName.Contains("ice") || tileName.Contains("winter"))
		{
			source = $"tileName:snow:{tile.name}";
			hint = SnowBiomeHint;
			return true;
		}

		if (tileName.Contains("ash") || tileName.Contains("waste") || tileName.Contains("dead") || tileName.Contains("ruin"))
		{
			source = $"tileName:ash:{tile.name}";
			hint = AshBiomeHint;
			return true;
		}

		if (tileName.Contains("lava") || tileName.Contains("volcan") || tileName.Contains("magma"))
		{
			source = $"tileName:lava:{tile.name}";
			hint = LavaBiomeHint;
			return true;
		}

		if (tileName.Contains("sand") || tileName.Contains("beach") || tileName.Contains("shore"))
		{
			source = $"tileName:beach:{tile.name}";
			hint = BeachBiomeHint;
			return true;
		}

		if (tileName.Contains("forest") || tileName.Contains("grass") || tileName.Contains("leaf") || tileName.Contains("wood"))
		{
			source = $"tileName:forest:{tile.name}";
			hint = ForestBiomeHint;
			return true;
		}

		source = string.Empty;
		hint = string.Empty;
		return false;
	}

	private void SpawnQuestItemOnMap()
	{
		if (spawnedItemInstance != null)
		{
			Destroy(spawnedItemInstance);
			spawnedItemInstance = null;
		}

		if (questItemPrefab == null)
		{
			Debug.LogWarning("[QUEST] Brak przypiętego Quest Item Prefab. Quest działa, ale item nie pojawi się na mapie.");
			return;
		}

		spawnedItemInstance = Instantiate(questItemPrefab, questSpawnWorldPosition, Quaternion.identity);
		spawnedItemInstance.name = targetItemName;

		SpriteRenderer spriteRenderer = spawnedItemInstance.GetComponent<SpriteRenderer>();
		if (spriteRenderer != null)
		{
			spriteRenderer.sprite = targetItemSprite;
		}

		PickableQuestItem pickup = spawnedItemInstance.GetComponent<PickableQuestItem>();
		if (pickup != null)
		{
			pickup.itemName = targetItemName;
		}
	}
}
