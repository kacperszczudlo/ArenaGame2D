using UnityEngine;

public class PlayerPositionTracker : MonoBehaviour
{
    private void Start()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        // Priorytet: jednorazowa pozycja powrotu z areny (z offsetem).
        if (GameManager.Instance.TryConsumeArenaReturnPosition(out Vector3 arenaReturnPosition))
        {
            transform.position = arenaReturnPosition;
            Debug.Log($"[MAPA] Wczytano pozycję powrotu z areny: {transform.position}");
        }
        // Fallback: standardowa zapisana pozycja.
        else if (GameManager.Instance.lastMapPosition != Vector3.zero)
        {
            transform.position = GameManager.Instance.lastMapPosition;
            Debug.Log($"[MAPA] Wczytano pozycję: {transform.position}");
        }

        GameManager.Instance.lastMapPosition = transform.position;
    }

    private void Update()
    {
        // Na bieżąco aktualizujemy pozycję w pamięci runtime.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.lastMapPosition = transform.position;
        }
    }

    private void OnDestroy()
    {
        SaveCurrentPositionIfPossible();
    }

    private void OnApplicationQuit()
    {
        SaveCurrentPositionIfPossible();
    }

    private void SaveCurrentPositionIfPossible()
    {
        if (GameManager.Instance != null && !GameManager.Instance.HasPendingArenaReturnPosition())
        {
            GameManager.Instance.SavePlayerPosition(transform.position);
        }
    }
}