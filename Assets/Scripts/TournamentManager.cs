using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class TournamentManager : MonoBehaviour
{
    [Header("Lista Przeciwników w Turnieju")]
    [Tooltip("Przeci¹gnij tu pliki EnemyData (np. Janusz). Pierwszy to indeks 0.")]
    public List<EnemyData> tournamentEnemies;

    [Header("UI Przeciwnika")]
    public Image enemyAvatar;
    public TextMeshProUGUI enemyInfoText;       // Np. "Janusz - Lvl 20"
    public TextMeshProUGUI enemyRewardText;     //"Nagroda: 100g | 50xp"

    [Header("UI Turnieju")]
    public TextMeshProUGUI progressText;        // Np. "Przeciwnik: 3/10"
    public TextMeshProUGUI pendingRewardsText;  // Np. "Zebrane ³upy: 300g"

    System.Collections.IEnumerator Start()
    {
        yield return null;

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

            enemyRewardText.text = $"Z³oto: +{currentEnemy.goldReward}  \nExp: {expText}";
        }

        UpdatePendingRewardsUI();
    }

    void UpdatePendingRewardsUI()
    {
        if (pendingRewardsText != null)
        {
            pendingRewardsText.text = $"Zebrane ³upy:\nZ³oto: {GameManager.Instance.pendingGold}\nExp: {GameManager.Instance.pendingXP}";
        }
    }

    public void OnClick_Fight()
    {
        int currentIndex = GameManager.Instance.currentTournamentIndex;
        if (currentIndex < tournamentEnemies.Count)
        {
            GameManager.Instance.currentEnemyToFight = tournamentEnemies[currentIndex];

            //Ustawiamy kontekst dla BattleManagera
            GameManager.Instance.sceneToLoadAfterBattle = "ArenaLobby"; // Wrócimy do Lobby
            GameManager.Instance.isTournamentBattle = true;             // U¿ywamy "tymczasowego worka" na ³upy

            SceneManager.LoadScene("FightScene");
        }
    }

    // Podpinane pod przycisk "WYCOFAJ SIÊ / OPUŒÆ ARENÊ"
    public void OnClick_Retreat()
    {
        // --- POPRAWKA: U¿ywamy twardego zapisu zamiast zwyk³ego "+=" ---
        GameManager.Instance.AddGold(GameManager.Instance.pendingGold);
        GameManager.Instance.AddGems(GameManager.Instance.pendingGems);

        if (GameManager.Instance.pendingXP > 0)
        {
            PlayerDataManager.Instance.AddExperience(GameManager.Instance.pendingXP);
            PlayerDataManager.Instance.SavePlayerData(); // Dodatkowy twardy zapis z expem!
        }

        GameManager.Instance.currentTournamentIndex = 0;
        GameManager.Instance.pendingGold = 0;
        GameManager.Instance.pendingXP = 0;
        GameManager.Instance.pendingGems = 0;

        SceneManager.LoadScene("MainScene");
    }
}