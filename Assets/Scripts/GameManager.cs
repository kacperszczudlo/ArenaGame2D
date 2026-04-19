using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private bool hasPendingArenaReturnPosition = false;
    private Vector3 pendingArenaReturnPosition = Vector3.zero;

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
            globalGold = PlayerPrefs.GetInt("GlobalGold", 100);
            tournamentGems = PlayerPrefs.GetInt("TournamentGems", 0);

            if (PlayerPrefs.HasKey("PlayerPosX"))
                {
                    float x = PlayerPrefs.GetFloat("PlayerPosX");
                    float y = PlayerPrefs.GetFloat("PlayerPosY");
                    float z = PlayerPrefs.GetFloat("PlayerPosZ");
                    lastMapPosition = new Vector3(x, y, z);
                }
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

    public void AddGems(int amount)
    {
        tournamentGems += amount;
        PlayerPrefs.SetInt("TournamentGems", tournamentGems);
        PlayerPrefs.Save();
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

    public void SavePlayerPosition(Vector3 currentPosition)
    {
        lastMapPosition = currentPosition;
        
        // Rozbijamy pozycję na X, Y, Z i zapisujemy na dysku
        PlayerPrefs.SetFloat("PlayerPosX", currentPosition.x);
        PlayerPrefs.SetFloat("PlayerPosY", currentPosition.y);
        PlayerPrefs.SetFloat("PlayerPosZ", currentPosition.z);
        PlayerPrefs.Save();
        
        Debug.Log($"[MAPA] Zapisano pozycję gracza: {currentPosition}");
    }

    public void SetArenaReturnPosition(Vector3 safePosition)
    {
        hasPendingArenaReturnPosition = true;
        pendingArenaReturnPosition = safePosition;

        // Trwale zapisujemy offset od razu, żeby po zamknięciu gry pozycja nie zniknęła.
        SavePlayerPosition(safePosition);

        Debug.Log($"[MAPA] Ustawiono pozycję powrotu z areny: {safePosition}");
    }

    public bool TryConsumeArenaReturnPosition(out Vector3 returnPosition)
    {
        if (!hasPendingArenaReturnPosition)
        {
            returnPosition = Vector3.zero;
            return false;
        }

        returnPosition = pendingArenaReturnPosition;
        pendingArenaReturnPosition = Vector3.zero;
        hasPendingArenaReturnPosition = false;
        return true;
    }

    public bool HasPendingArenaReturnPosition()
    {
        return hasPendingArenaReturnPosition;
    }
}