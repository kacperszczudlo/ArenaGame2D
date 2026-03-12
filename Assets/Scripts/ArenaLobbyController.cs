using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaLobbyController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartFight()
    {
        // Sprawdzamy, czy ustawiliœmy kogoœ do walki
        if (GameManager.Instance.currentEnemyToFight != null && GameManager.Instance.currentPlayerPrefab != null)
        {
            Debug.Log("£adujê Arenê...");
            // Upewnij siê, ¿e Twoja scena walki nazywa siê DOK£ADNIE "FightScene" w Unity
            SceneManager.LoadScene("FightScene");
        }
        else
        {
            Debug.LogWarning("Nie przypisa³eœ Gracza lub Wroga w GameManagerze!");
        }
    }
}
