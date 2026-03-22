using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))] 
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
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
        if (image != null && itemData != null)
        {
            image.sprite = itemData.icon; 
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        
        // Zmieniamy rodzica na Canvas podczas przeciągania
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
        // Tutaj był błąd! Dodanie parametru 'false' zmusza obiekt do porzucenia "świata" i idealnego dopasowania się do krawędzi rodzica (Slotu).
        transform.SetParent(parentAfterDrag, false);
        
        if (image != null) image.raycastTarget = true;

        // Wymuszamy wyśrodkowanie wewnątrz slotu (Snap to Center)
        RectTransform rect = GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0); 
        rect.anchorMax = new Vector2(1, 1); 
        rect.offsetMin = Vector2.zero; 
        rect.offsetMax = Vector2.zero;
        rect.anchoredPosition = Vector2.zero; 
    }
}