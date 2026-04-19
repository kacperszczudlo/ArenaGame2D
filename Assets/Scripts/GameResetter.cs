using UnityEngine;
using UnityEngine.SceneManagement;

public class GameResetter : MonoBehaviour
{
    public void ResetEntireGame()
    {
        // 1. Kasujemy absolutnie wszystkie sejwy z dysku (w tym zapis Ekwipunku)
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // 2. "Zabijamy" nieœmiertelne mened¿ery, ¿eby wyczyœciæ im pamiêæ RAM!
        if (PlayerDataManager.Instance != null) Destroy(PlayerDataManager.Instance.gameObject);
        if (GameManager.Instance != null) Destroy(GameManager.Instance.gameObject);

        // Jeœli Twój InventoryManager/SaveSystem te¿ jest nieœmiertelny, zabijamy go:
        if (InventorySaveSystem.Instance != null) Destroy(InventorySaveSystem.Instance.gameObject);
        if (InventoryManager.Instance != null) Destroy(InventoryManager.Instance.gameObject);

        Debug.Log("<color=red>HARD RESET! Mened¿ery zabite, sejwy usuniête, plecak pusty.</color>");

        // 3. --- NOWOŒÆ: Zamiast odœwie¿aæ Menu, od razu rzucamy gracza na Mapê! ---
        SceneManager.LoadScene("MainScene");
    }
}