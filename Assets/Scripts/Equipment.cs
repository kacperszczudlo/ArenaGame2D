using UnityEngine;

public class Equipment : MonoBehaviour 
{
    [Header("UI Elements")]
    [SerializeField] private GameObject equipmentWindow; 
    
    public void OpenWindow()
    { 

        if (equipmentWindow != null)
        {
            equipmentWindow.SetActive(true); 
        }

    }

    public void CloseWindow()
    { 

        if (equipmentWindow != null)
        {
            equipmentWindow.SetActive(false); 
        }

    }
}