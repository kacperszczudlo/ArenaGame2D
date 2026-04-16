using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class QuestManager : MonoBehaviour
{
	public static QuestManager Instance { get; private set; }

	[Header("Tilemapy dozwolone")]
	public Tilemap groundMap;
	public Tilemap stairsMap;
	public Tilemap bridgeMap;

	[Header("Tilemapy blokujące")]
	public Tilemap[] blockingMaps;

	[Header("Przeciwnik do obliczenia nagrody")]
	public EnemyData currentNextEnemy;

	[Header("Źródło ikon questowych")]
	[SerializeField] private string questItemsResourcesFolder = "QuestItems";

	[Header("Prefab przedmiotu questowego")]
	public GameObject questItemPrefab;

	[Header("Wskazówki biomów")]
	[SerializeField] private float snowBiomeMinY = 15f;
	[SerializeField] private float ashBiomeMinY = 0f;
	[SerializeField] private float lavaBiomeMinY = -10f;
	[SerializeField] private float forestBiomeMinX = 10f;
	[SerializeField] private string snowBiomeHint = "w Śnieżnej Twierdzy";
	[SerializeField] private string ashBiomeHint = "na Popielnych Pustkowiach";
	[SerializeField] private string lavaBiomeHint = "w Krainie Lawy";
	[SerializeField] private string forestBiomeHint = "w Lesie";
	[SerializeField] private string beachBiomeHint = "na Piaszczystej Plaży";

	[HideInInspector] public bool isQuestActive;
	[HideInInspector] public bool hasCollectedItem;
	[HideInInspector] public string targetItemName;
	[HideInInspector] public Sprite targetItemSprite;
	[HideInInspector] public string biomeHint;
	[HideInInspector] public int calculatedGoldReward;

	public Vector3Int questSpawnCell { get; private set; }
	public Vector3 questSpawnWorldPosition { get; private set; }

	private GameObject spawnedItemInstance;

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

		if (GameManager.Instance != null && GameManager.Instance.currentEnemyToFight != null)
		{
			currentNextEnemy = GameManager.Instance.currentEnemyToFight;
		}

		Sprite[] allItems = Resources.LoadAll<Sprite>(questItemsResourcesFolder);
		if (allItems == null || allItems.Length == 0)
		{
			Debug.LogError($"[QUEST] Nie znaleziono ikon w Resources/{questItemsResourcesFolder}.");
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
		biomeHint = ResolveBiomeHint(questSpawnWorldPosition);
		calculatedGoldReward = CalculateQuestReward();
		isQuestActive = true;
		hasCollectedItem = false;

		SpawnQuestItemOnMap();

		Debug.Log($"[QUEST] Wylosowano zadanie. Przynieś: {targetItemName}. Szukaj {biomeHint}. Nagroda: {calculatedGoldReward}g. Spawn: {questSpawnWorldPosition}");
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
		EnemyData rewardSource = currentNextEnemy;
		if (GameManager.Instance != null && GameManager.Instance.currentEnemyToFight != null)
		{
			rewardSource = GameManager.Instance.currentEnemyToFight;
		}

		if (rewardSource == null)
		{
			return 10;
		}

		return Mathf.RoundToInt(rewardSource.goldReward / 4f);
	}

	private bool TryFindValidSpawnLocation(out Vector3Int spawnCell)
	{
		HashSet<Vector3Int> validCells = new HashSet<Vector3Int>();

		CollectValidCellsFromMap(groundMap, validCells);
		CollectValidCellsFromMap(stairsMap, validCells);
		CollectValidCellsFromMap(bridgeMap, validCells);

		if (validCells.Count == 0)
		{
			spawnCell = default;
			Debug.LogWarning("[QUEST] Nie udało się znaleźć poprawnego miejsca na spawn przedmiotu.");
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

	private void CollectValidCellsFromMap(Tilemap tilemap, HashSet<Vector3Int> validCells)
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

			if (IsBlocked(cell))
			{
				continue;
			}

			validCells.Add(cell);
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
			if (blockingMap != null && blockingMap.HasTile(cell))
			{
				return true;
			}
		}

		return false;
	}

	private Vector3 GetWorldPosition(Vector3Int cell)
	{
		Tilemap referenceMap = groundMap != null ? groundMap : stairsMap != null ? stairsMap : bridgeMap;
		if (referenceMap != null)
		{
			return referenceMap.GetCellCenterWorld(cell);
		}

		return Vector3.zero;
	}

	private string ResolveBiomeHint(Vector3 worldPosition)
	{
		if (worldPosition.y >= snowBiomeMinY)
		{
			return snowBiomeHint;
		}

		if (worldPosition.y >= ashBiomeMinY)
		{
			return ashBiomeHint;
		}

		if (worldPosition.x >= forestBiomeMinX)
		{
			return forestBiomeHint;
		}

		if (worldPosition.y >= lavaBiomeMinY)
		{
			return lavaBiomeHint;
		}

		return beachBiomeHint;
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
