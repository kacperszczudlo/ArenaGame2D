using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Przejœcia miêdzy scenami")]
    public GameObject currentPlayerPrefab;
    public EnemyData currentEnemyToFight;
    public Vector3 lastMapPosition; // Zapamiêta koordynaty X, Y, Z

    [Header("Kontekst Walki")]
    public string sceneToLoadAfterBattle = "ArenaLobby";
    public bool isTournamentBattle = true;

    [Header("Globalne Zasoby")]
    public int globalGold = 0;
    public int tournamentGems = 0;

    [Header("Stan Turnieju")]
    public int currentTournamentIndex = 0;
    public int pendingGold = 0; // Z³oto w "wirtualnym worku" 
    public int pendingXP = 0;   // Doœwiadczenie w "wirtualnym worku"
    public int pendingGems = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }
}