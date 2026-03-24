using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Przej�cia mi�dzy scenami")]
    public GameObject currentPlayerPrefab;
    public EnemyData currentEnemyToFight;
    public Vector3 lastMapPosition; // Zapami�ta koordynaty X, Y, Z

    [Header("Kontekst Walki")]
    public string sceneToLoadAfterBattle = "ArenaLobby";
    public bool isTournamentBattle = true;

    [Header("Globalne Zasoby")]
    public int globalGold = 0;
    public int tournamentGems = 0;

    [Header("Stan Turnieju")]
    public int currentTournamentIndex = 0;
    public int pendingGold = 0; // Z�oto w "wirtualnym worku" 
    public int pendingXP = 0;   // Do�wiadczenie w "wirtualnym worku"
    public int pendingGems = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
            globalGold = PlayerPrefs.GetInt("GlobalGold", 1500);
            tournamentGems = PlayerPrefs.GetInt("TournamentGems", 0);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- FUNKCJE EKONOMII (Dodane dla Kowala i Ekwipunku) ---
    public void AddGold(int amount)
    {
        globalGold += amount;
        PlayerPrefs.SetInt("GlobalGold", globalGold);
        PlayerPrefs.Save();
        Debug.Log($"[BANK] Dodano {amount} złota. Aktualny stan: {globalGold}");
    }

    public bool SpendGold(int amount)
    {
        if (globalGold >= amount)
        {
            globalGold -= amount;
            PlayerPrefs.SetInt("GlobalGold", globalGold);
            PlayerPrefs.Save();
            Debug.Log($"[BANK] Wydano {amount} złota. Pozostało: {globalGold}");
            return true;
        }
        Debug.LogWarning("[BANK] Brak wystarczającej ilości złota!");
        return false;
    }
}