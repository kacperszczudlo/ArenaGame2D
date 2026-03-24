using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))] 
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("Dane (Wypełniane dynamicznie w grze)")]
    public EquipmentItemData itemData;
    
    [HideInInspector] public Transform parentAfterDrag;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Setup(EquipmentItemData data)
    {
        itemData = data;
        
        // Zabezpieczenie przed brakiem komponentu Image
        if (image == null) image = GetComponent<Image>();

        if (image != null && itemData != null)
        {
            if (itemData.icon != null) 
            {
                image.sprite = itemData.icon; 
                image.color = Color.white; // Wymuszenie 100% widoczności
            }
            else 
            {
                // Próba ratunku: jeśli ikona jest null, spróbuj załadować ją z nazwy
                if (!string.IsNullOrEmpty(itemData.iconName))
                {
                    itemData.icon = Resources.Load<Sprite>("BlacksmithShop/" + itemData.iconName);
                    if (itemData.icon != null)
                    {
                        image.sprite = itemData.icon;
                        image.color = Color.white;
                        return; // Udało się uratować
                    }
                }
                
                // Jeśli definitywnie nie ma ikony - ukrywamy kwadrat
                image.color = new Color(1f, 1f, 1f, 0f); 
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        if (image != null) image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag, false);
        if (image != null) image.raycastTarget = true;

        RectTransform rect = GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0); 
        rect.anchorMax = new Vector2(1, 1); 
        rect.offsetMin = Vector2.zero; 
        rect.offsetMax = Vector2.zero;
        rect.anchoredPosition = Vector2.zero; 
    }

    // --- SPRZEDAŻ (PRAWY KLIK) ---
    // --- SPRZEDAŻ (PRAWY KLIK) ---
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ItemSlot slot = transform.parent.GetComponent<ItemSlot>();
            
            if (slot != null && !slot.isEquippedSlot)
            {
                // WYSYŁAMY ZŁOTO DO GAMEMANAGERA
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddGold(itemData.sellPrice);
                }

                BlacksmithShopController shop = Object.FindFirstObjectByType<BlacksmithShopController>(FindObjectsInactive.Include);
                if (shop != null) 
                { 
                    shop.UpdateGoldUI(); 
                }
                
                transform.SetParent(null);
                Destroy(gameObject);
                
                if(InventorySaveSystem.Instance != null)
                {
                    InventorySaveSystem.Instance.SaveInventory();
                }
            }
            else
            {
                Debug.LogWarning("[HANDEL] Zdejmij przedmiot z postaci zanim go sprzedasz!");
            }
        }
    }
}