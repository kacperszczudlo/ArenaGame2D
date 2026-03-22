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

    // Podmieñ star¹ funkcjê Start() na to:
    System.Collections.IEnumerator Start()
    {
        // Czekamy JEDN¥ KLATKÊ, a¿ GameManagery siê dogadaj¹ i stary zniszczy nowego
        yield return null;

        // Teraz ³adujemy UI, czytaj¹c z prawid³owego, ocala³ego GameManagera
        UpdateLobbyUI();
    }

    void UpdateLobbyUI()
    {
        int currentIndex = GameManager.Instance.currentTournamentIndex;

        if (currentIndex >= tournamentEnemies.Count)
        {
            if (enemyInfoText != null) enemyInfoText.text = "Mistrz Areny Pokonany!";
            if (progressText != null) progressText.text = "Turniej Zakoñczony!";
            if (enemyRewardText != null) enemyRewardText.text = "";
            if (enemyAvatar != null) enemyAvatar.gameObject.SetActive(false);
            UpdatePendingRewardsUI();
            return;
        }

        EnemyData currentEnemy = tournamentEnemies[currentIndex];

        if (enemyAvatar != null) enemyAvatar.sprite = currentEnemy.avatarImage;
        if (enemyInfoText != null) enemyInfoText.text = $"{currentEnemy.enemyName} - Lvl {currentEnemy.level}";
        if (progressText != null) progressText.text = $"Przeciwnik: {currentIndex + 1} / {tournamentEnemies.Count}";

        if (enemyRewardText != null)
        {
            int actualExp = (currentEnemy.level > PlayerDataManager.Instance.currentLevel) ? currentEnemy.expReward : 0;
            string expText = actualExp > 0 ? $"+{actualExp}" : "<color=#888888>0 (Brak wyzwania!)</color>";

            // Czysty tekst nagrody - ZERO spoilerów o gemach!
            enemyRewardText.text = $"Z³oto: +{currentEnemy.goldReward}  \nExp: {expText}";
        }

        UpdatePendingRewardsUI();
    }

    void UpdatePendingRewardsUI()
    {
        // Worek pokazuje tylko z³oto i exp
        if (pendingRewardsText != null)
        {
            pendingRewardsText.text = $"Zebrane ³upy:\nZ³oto: {GameManager.Instance.pendingGold}\nExp: {GameManager.Instance.pendingXP}";
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
        GameManager.Instance.globalGold += GameManager.Instance.pendingGold;

        // Zgarniamy gemy z worka w tle (gracz tego nie widzi w lobby, ale dostaje je na konto!)
        GameManager.Instance.tournamentGems += GameManager.Instance.pendingGems;

        if (GameManager.Instance.pendingXP > 0)
        {
            PlayerDataManager.Instance.AddExperience(GameManager.Instance.pendingXP);
        }

        GameManager.Instance.currentTournamentIndex = 0;
        GameManager.Instance.pendingGold = 0;
        GameManager.Instance.pendingXP = 0;
        GameManager.Instance.pendingGems = 0; // Czyœcimy gemy

        SceneManager.LoadScene("MainScene");
    }
}