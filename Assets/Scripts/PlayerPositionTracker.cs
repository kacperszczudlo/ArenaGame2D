using UnityEngine;

public class PlayerPositionTracker : MonoBehaviour
{
    private void Start()
    {
        // Jeśli GameManager ma zapisaną pozycję (inną niż 0,0,0), teleportuj tam gracza
        if (GameManager.Instance != null && GameManager.Instance.lastMapPosition != Vector3.zero)
        {
            transform.position = GameManager.Instance.lastMapPosition;
            Debug.Log($"[MAPA] Wczytano pozycję: {transform.position}");
        }
    }

    private void Update()
    {
        // Na bieżąco aktualizujemy pozycję w GameManagerze
        if (GameManager.Instance != null)
        {
            GameManager.Instance.lastMapPosition = transform.position;
        }
    }

    private void OnDestroy()
    {
        // Przy zamykaniu gry lub zmianie sceny zapisujemy ostatecznie do PlayerPrefs
        if (GameManager.Instance != null)
        {
            PlayerPrefs.SetFloat("PlayerPosX", transform.position.x);
            PlayerPrefs.SetFloat("PlayerPosY", transform.position.y);
            PlayerPrefs.SetFloat("PlayerPosZ", transform.position.z);
            PlayerPrefs.Save();
        }
    }
}