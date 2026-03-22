using UnityEngine;

public class BlacksmithInteractable : MonoBehaviour 
{
    private BlacksmithShopController shopController;

    private void Start()
    {
        // Automatycznie znajdujemy sklep na scenie 
        // (nawet jeśli domyślnie jest wyłączony, FindFirstObjectByType z przełącznikiem FindObjectsInactive zadziała w nowym Unity 6000)
        shopController = Object.FindFirstObjectByType<BlacksmithShopController>(FindObjectsInactive.Include);
        
        if (shopController == null)
        {
            Debug.LogError("Nie znaleziono BlacksmithShopController na scenie! Upewnij się, że obiekt BlacksmithShop znajduje się w hierarchii Canvasu.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && shopController != null)
        {
            shopController.OpenShop();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && shopController != null)
        {
            shopController.CloseShop();
        }
    }
}