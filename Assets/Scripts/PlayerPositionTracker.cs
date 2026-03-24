using UnityEngine;

public class PlayerPositionTracker : MonoBehaviour
{
    private void Start()
    {
        // 1. Kiedy postać pojawia się na scenie, pyta GameManagera o zapisaną pozycję
        if (GameManager.Instance != null && PlayerPrefs.HasKey("PlayerPosX"))
        {
            transform.position = GameManager.Instance.lastMapPosition;
            Debug.Log($"[MAPA] Wczytano i przeniesiono gracza na pozycję: {transform.position}");
        }
    }

    // 2. Automatyczny zapis pozycji podczas wyłączania gry
    private void OnApplicationQuit()
    {
        SaveCurrentPosition();
    }

    // 3. Automatyczny zapis pozycji, gdy np. przechodzisz do innej sceny (do walki)
    private void OnDestroy()
    {
        SaveCurrentPosition();
    }

    private void SaveCurrentPosition()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SavePlayerPosition(transform.position);
        }
    }
}