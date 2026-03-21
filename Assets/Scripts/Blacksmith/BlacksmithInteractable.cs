using UnityEngine;

public class BlacksmithInteractable : MonoBehaviour 
{
    [SerializeField] private BlacksmithShopController shopController;

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