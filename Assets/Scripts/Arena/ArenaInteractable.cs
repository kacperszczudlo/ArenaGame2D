using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaInteractable : MonoBehaviour
{
    [Header("Ustawienia Wej�cia")]
    [Tooltip("Wpisz tu dok�adn� nazw� sceny, do kt�rej idziemy")]
    public string arenaLobbySceneName = "ArenaLobby";

    [Header("Pozycja Powrotu")]
    [Tooltip("Offset pozycji gracza po powrocie z areny, aby unikn�� ponownego triggera")]
    public Vector3 returnPositionOffset = new Vector3(0f, -2f, 0f);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Sprawdzamy, czy to gracz wszed� w stref�
        if (collision.CompareTag("Player"))
        {
            // 1. Zapami�tujemy pozycj� gracza z offsetem do bezpiecznego powrotu
            if (GameManager.Instance != null)
            {
                Vector3 safePosition = collision.transform.position + returnPositionOffset;
                GameManager.Instance.SetArenaReturnPosition(safePosition);
            }

            // 2. �adujemy scen� z turniejem
            SceneManager.LoadScene(arenaLobbySceneName);
        }
    }
}