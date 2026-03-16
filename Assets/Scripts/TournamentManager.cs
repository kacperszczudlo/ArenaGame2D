using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class TournamentManager : MonoBehaviour
{
    [Header("Lista Przeciwników w Turnieju")]
    [Tooltip("Przeci¹gnij tu pliki EnemyData (np. Janusz, Szkielet, Smok). Pierwszy to indeks 0.")]
    public List<EnemyData> tournamentEnemies;

    [Header("UI Przeciwnika")]
    public Image enemyAvatar;
    public TextMeshProUGUI enemyInfoText;       // Np. "Janusz - Lvl 20"
    public TextMeshProUGUI enemyRewardText;     // NOWOŒÆ: "Nagroda: 100g | 50xp"

    [Header("UI Turnieju")]
    public TextMeshProUGUI progressText;        // Np. "Przeciwnik: 3/10"
    public TextMeshProUGUI pendingRewardsText;  // Np. "Zebrane ³upy: 300g"

    void Start()
    {
        UpdateLobbyUI();
    }

    void UpdateLobbyUI()
    {
        int currentIndex = GameManager.Instance.currentTournamentIndex;

        // Sprawdzamy czy gracz nie pokona³ ju¿ wszystkich!
        if (currentIndex >= tournamentEnemies.Count)
        {
            if (enemyInfoText != null) enemyInfoText.text = "Mistrz Areny Pokonany!";
            if (progressText != null) progressText.text = "Turniej Zakoñczony!";
            if (enemyRewardText != null) enemyRewardText.text = "";
            if (enemyAvatar != null) enemyAvatar.gameObject.SetActive(false);
            UpdatePendingRewardsUI();
            return;
        }

        // Pobieramy obecnego wroga z listy
        EnemyData currentEnemy = tournamentEnemies[currentIndex];

        // Aktualizujemy informacje o przeciwniku
        if (enemyAvatar != null) enemyAvatar.sprite = currentEnemy.avatarImage;
        if (enemyInfoText != null) enemyInfoText.text = $"{currentEnemy.enemyName} - Lvl {currentEnemy.level}";
        if (progressText != null) progressText.text = $"Przeciwnik: {currentIndex + 1} / {tournamentEnemies.Count}";

        // NOWOŒÆ: Pokazujemy potencjaln¹ nagrodê za tego konkretnego wroga
        if (enemyRewardText != null)
        {
            enemyRewardText.text = $"Zwyciêstwo da Ci:\nZ³oto: +{currentEnemy.goldReward} | Exp: +{currentEnemy.expReward}";
        }

        UpdatePendingRewardsUI();
    }

    void UpdatePendingRewardsUI()
    {
        // Pokazujemy to, co gracz ju¿ uzbiera³ w tym podejœciu i co bezpiecznie zabierze
        if (pendingRewardsText != null)
        {
            pendingRewardsText.text = $"Zebrane ³upy (Twoje niezale¿nie od wyniku):\nZ³oto: {GameManager.Instance.pendingGold}\nExp: {GameManager.Instance.pendingXP}";
        }
    }

    // Podpinane pod przycisk "WALCZ"
    public void OnClick_Fight()
    {
        int currentIndex = GameManager.Instance.currentTournamentIndex;
        if (currentIndex < tournamentEnemies.Count)
        {
            GameManager.Instance.currentEnemyToFight = tournamentEnemies[currentIndex];

            // --- NOWOŒÆ: Ustawiamy kontekst dla BattleManagera! ---
            GameManager.Instance.sceneToLoadAfterBattle = "ArenaLobby"; // Wrócimy do Lobby
            GameManager.Instance.isTournamentBattle = true;             // U¿ywamy "tymczasowego worka" na ³upy

            SceneManager.LoadScene("FightScene");
        }
    }

    // Podpinane pod przycisk "WYCOFAJ SIÊ / OPUŒÆ ARENÊ"
    public void OnClick_Retreat()
    {
        // 1. Zgarniamy ³upy do g³ównej puli!
        GameManager.Instance.globalGold += GameManager.Instance.pendingGold;

        // --- NOWOŒÆ: Przekazujemy Exp do PlayerDataManagera! ---
        PlayerDataManager.Instance.currentExperience += GameManager.Instance.pendingXP;

        Debug.Log($"<color=green>Wycofujesz siê! Zgarniasz {GameManager.Instance.pendingGold} z³ota i {GameManager.Instance.pendingXP} expa.</color>");

        // 2. Resetujemy stan turnieju
        GameManager.Instance.currentTournamentIndex = 0;
        GameManager.Instance.pendingGold = 0;
        GameManager.Instance.pendingXP = 0;

        UpdateLobbyUI();
    }
}