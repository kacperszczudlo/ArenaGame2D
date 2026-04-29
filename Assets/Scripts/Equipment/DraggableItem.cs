using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))] 
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Potwierdzenia")]
    [SerializeField] private SellConfirmationUI sellConfirmationDialog;

    [Header("Dane (Wypełniane dynamicznie w grze)")]
    public EquipmentItemData itemData;
    // Per-instance upgrade level (nie modyfikuje oryginalnego ScriptableObject)
    public int upgradeLevel = 0;
    // Per-stat upgrade allocations (index matches UpgradeManager dropdown)
    public const int UPGRADE_STAT_COUNT = 13;
    public System.Collections.Generic.List<int> upgradePoints = new System.Collections.Generic.List<int>(new int[UPGRADE_STAT_COUNT]);
    
    // Mirror support: if this object is a visual mirror, forward drags to original
    [HideInInspector] public bool isMirror = false;
    [HideInInspector] public DraggableItem originalSource = null;

    // When true, this original item is being dragged visually by a mirror.
    // Original should skip moving its own transform while this is set.
    [HideInInspector] public bool isDraggedExternally = false;

    [HideInInspector] public Transform parentAfterDrag;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Setup(EquipmentItemData data)
    {
        itemData = data;
        if (image == null) image = GetComponent<Image>();

        // Freshly spawned items must start with their own upgrade state.
        // Loaded items restore their state right after Setup() in InventorySaveSystem.
        upgradeLevel = 0;
        if (upgradePoints == null) upgradePoints = new System.Collections.Generic.List<int>(new int[UPGRADE_STAT_COUNT]);
        else
        {
            upgradePoints.Clear();
            for (int i = 0; i < UPGRADE_STAT_COUNT; i++) upgradePoints.Add(0);
        }

        // Ensure upgradePoints list has expected length
        if (upgradePoints.Count < UPGRADE_STAT_COUNT)
        {
            int need = UPGRADE_STAT_COUNT - upgradePoints.Count;
            for (int i = 0; i < need; i++) upgradePoints.Add(0);
        }

        if (image != null && itemData != null)
        {
            if (itemData.icon != null) 
            {
                image.sprite = itemData.icon; 
                image.color = Color.white; 
            }
            else 
            {
                if (!string.IsNullOrEmpty(itemData.iconName))
                {
                    itemData.icon = Resources.Load<Sprite>("BlacksmithShop/" + itemData.iconName);
                    if (itemData.icon != null)
                    {
                        image.sprite = itemData.icon;
                        image.color = Color.white;
                        return;
                    }
                }
                image.color = new Color(1f, 1f, 1f, 0f); 
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isMirror && originalSource != null)
        {
            // Mark original so it doesn't move its transform when receiving drag events
            originalSource.isDraggedExternally = true;

            // Prepare visual mirror for dragging
            parentAfterDrag = transform.parent;
            transform.SetParent(transform.root);
            transform.SetAsLastSibling();
            if (image != null) image.raycastTarget = false;

            // Forward drag event to original so internal state (if any) updates
            ExecuteEvents.Execute(originalSource.gameObject, eventData, ExecuteEvents.beginDragHandler);
            return;
        }

        // If this original is being dragged externally by a mirror, skip moving it here
        if (isDraggedExternally)
        {
            parentAfterDrag = transform.parent;
            return;
        }

        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        if (image != null) image.raycastTarget = false;

        if (ItemTooltip.Instance != null) ItemTooltip.Instance.HideTooltip(); 
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isMirror && originalSource != null)
        {
            // forward logic to original, but move the mirror visual so player sees it
            ExecuteEvents.Execute(originalSource.gameObject, eventData, ExecuteEvents.dragHandler);
            transform.position = Input.mousePosition;
            return;
        }

        if (isDraggedExternally)
        {
            // original should not follow mouse when dragged by mirror
            return;
        }

        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isMirror && originalSource != null)
        {
            // forward end-drag to original so it can finalize logic
            ExecuteEvents.Execute(originalSource.gameObject, eventData, ExecuteEvents.endDragHandler);

            // release original external-drag flag
            originalSource.isDraggedExternally = false;

            // restore mirror visual to its parent (visual placement is handled by UpgradeDropZone or caller)
            transform.SetParent(parentAfterDrag, false);
            if (image != null) image.raycastTarget = true;

            RectTransform rect = GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
            return;
        }

        if (isDraggedExternally)
        {
            // If original received end-drag forwarded, do nothing here
            isDraggedExternally = false;
            return;
        }

        transform.SetParent(parentAfterDrag, false);
        if (image != null) image.raycastTarget = true;

        RectTransform rectNormal = GetComponent<RectTransform>();
        rectNormal.anchorMin = new Vector2(0, 0);
        rectNormal.anchorMax = new Vector2(1, 1);
        rectNormal.offsetMin = Vector2.zero;
        rectNormal.offsetMax = Vector2.zero;
        rectNormal.anchoredPosition = Vector2.zero;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ItemSlot slot = transform.parent.GetComponent<ItemSlot>();
            
            if (slot != null && !slot.isEquippedSlot)
            {
                if (sellConfirmationDialog == null)
                {
                    sellConfirmationDialog = Object.FindFirstObjectByType<SellConfirmationUI>(FindObjectsInactive.Include);
                }

                if (sellConfirmationDialog == null)
                {
                    Debug.LogError("[HANDEL] Brak SellConfirmationUI w scenie. Sprzedaż anulowana.");
                    return;
                }

                string itemName = itemData != null ? itemData.itemName : gameObject.name;
                int price = itemData != null ? itemData.sellPrice : 0;
                sellConfirmationDialog.Show(itemName, price, SellCurrentItem);
            }
            else
            {
                Debug.LogWarning("[HANDEL] Zdejmij przedmiot z postaci zanim go sprzedasz!");
            }
        }
    }

    private void SellCurrentItem()
    {
        if (GameManager.Instance != null && itemData != null)
        {
            GameManager.Instance.AddGold(itemData.sellPrice);
        }

        BlacksmithShopController shop = Object.FindFirstObjectByType<BlacksmithShopController>(FindObjectsInactive.Include);
        if (shop != null)
        {
            shop.UpdateGoldUI();
        }

        transform.SetParent(null);
        if (ItemTooltip.Instance != null) ItemTooltip.Instance.HideTooltip();

        Destroy(gameObject);

        if (InventorySaveSystem.Instance != null)
        {
            InventorySaveSystem.Instance.SaveInventory();
        }
    }

    // --- POPRAWIONE FUNKCJE HOVER ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        // SZPIEG 1: Sprawdzamy czy Unity w ogóle widzi, że najechaliśmy na przedmiot
        if (itemData == null)
        {
            return;
        }

        Debug.Log($"[MYSZ] Najechano na przedmiot: {itemData.itemName}");

        if (ItemTooltip.Instance != null)
        {
            DraggableItem source = (isMirror && originalSource != null) ? originalSource : this;
            ItemTooltip.Instance.ShowTooltip(itemData, source);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Chowamy tooltip
        if (ItemTooltip.Instance != null)
        {
            ItemTooltip.Instance.HideTooltip();
        }
    }
}