using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class MenuMain : MonoBehaviour 
{
    public void PlayGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void QuitGame()
    {

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}