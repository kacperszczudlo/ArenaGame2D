using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Przejœcia miêdzy scenami")]
    public GameObject currentPlayerPrefab; // Zaktualizowane: Cia³o wybrane przez gracza
    public EnemyData currentEnemyToFight;  // Kogo idziemy biæ?
    public Vector3 lastMapPosition; // Zapamiêta koordynaty X, Y, Z przed wejœciem w krzaki!

    [Header("Kontekst Walki")]
    public string sceneToLoadAfterBattle = "ArenaLobby"; // Gdzie mamy wróciæ po walce?
    public bool isTournamentBattle = true;

    [Header("Globalne Zasoby")]
    public int globalGold = 0;

    [Header("Stan Turnieju")]
    public int currentTournamentIndex = 0; // Który to przeciwnik z rzêdu? (0 to pierwszy)
    public int pendingGold = 0; // Z³oto w "wirtualnym worku" 
    public int pendingXP = 0;   // Doœwiadczenie w "wirtualnym worku"

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ten skrypt te¿ przetrwa zmianê sceny!
        }
        else
        {
            Destroy(gameObject);
        }
    }
}