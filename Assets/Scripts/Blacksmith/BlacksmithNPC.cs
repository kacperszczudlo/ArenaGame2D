using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// DODALIŚMY IPointerClickHandler
public class BlacksmithNPC : MonoBehaviour, IPointerClickHandler 
{
    [Header("UI Dialogu (Z Canvasa)")]
    public GameObject dialogBox;
    public Button btnUpgrade;
    public Button btnShop;
    public Button btnLeave;

    [Header("Sklep (Z Canvasa)")]
    public BlacksmithShopController shopController;

    [Header("Okna Ekwipunku")]
    public GameObject upgradeWindow;      // Lewe okno (Kowadło - Upgrade_Window)
    public GameObject inventoryWindow;    // Prawe okno (Oryginalny Equipment_Window)
    
    [Header("Dymek nad postacią")]
    public GameObject chatBubble;

    private void Start()
    {
        if (btnShop != null) btnShop.onClick.AddListener(OpenShop);
        if (btnUpgrade != null) btnUpgrade.onClick.AddListener(OpenUpgradeSystem);
        if (btnLeave != null) btnLeave.onClick.AddListener(CloseDialogue);
    }

    // ZAMIAST OnMouseDown, UŻYWAMY NOWOCZESNEGO OnPointerClick
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("[MYSZ] Nowy system UI wykrył kliknięcie w kowala!");
        OpenDialogue();
    }

    private void OpenDialogue()
    {
        if (dialogBox != null) dialogBox.SetActive(true);
        if (chatBubble != null) chatBubble.SetActive(false); 
    }

    private void CloseDialogue()
    {
        if (dialogBox != null) dialogBox.SetActive(false);
        if (chatBubble != null) chatBubble.SetActive(true); 
    }

    private void OpenShop()
    {
        CloseDialogue(); 
        if (shopController != null) 
        {
            shopController.OpenShop(); 
        }
    }

    private void OpenUpgradeSystem()
    {
        CloseDialogue();

        // Otwieramy TYLKO Upgrade_Window - zawiera on już Grid_Inventory z przedmiotami
        if (upgradeWindow != null) upgradeWindow.SetActive(true);
        
        // Equipment_Window pozostaje zamknięty - wszystko co potrzebne jest już w Upgrade_Window!
    }
}