using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaLobbyController : MonoBehaviour
{
    public void StartFight()
    {
        // Sprawdzamy, czy ustawiliœmy kogoœ do walki
        if (GameManager.Instance.currentEnemyToFight != null && GameManager.Instance.currentPlayerPrefab != null)
        {
            Debug.Log("£adujê Arenê...");
            SceneManager.LoadScene("FightScene");
        }
        else
        {
            Debug.LogWarning("Nie przypisa³eœ Gracza lub Wroga w GameManagerze!");
        }
    }
}
