using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaInteractable : MonoBehaviour
{
    [Header("Ustawienia Wejœcia")]
    [Tooltip("Wpisz tu dok³adn¹ nazwê sceny, do której idziemy")]
    public string arenaLobbySceneName = "ArenaLobby";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Sprawdzamy, czy to gracz wszed³ w strefê
        if (collision.CompareTag("Player"))
        {
            // 1. Zapamiêtujemy pozycjê gracza w GameManagerze, ale z OFFSETEM
            if (GameManager.Instance != null)
            {
                // Zapisujemy pozycjê gracza, ale odejmujemy 2 od osi Y 
                Vector3 safePosition = collision.transform.position + new Vector3(0f, -2f, 0f);

                GameManager.Instance.lastMapPosition = safePosition;
            }

            // 2. £adujemy scenê z turniejem
            SceneManager.LoadScene(arenaLobbySceneName);
        }
    }
}