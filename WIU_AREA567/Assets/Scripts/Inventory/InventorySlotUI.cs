using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IDropHandler
{
    [Header("UI References")]
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private GameObject selectionBorder;

    private int slotIndex;
    public int SlotIndex => slotIndex;

    private ItemInstance currentItem;
    private ItemTooltipUI tooltip;
    private Inventory inventory;
    private Image dragIcon;

    public void Initialise(int index, ItemTooltipUI tooltipUI,
        Inventory inventoryRef, Image dragIconRef)
    {
        slotIndex = index;
        tooltip = tooltipUI;
        inventory = inventoryRef;
        dragIcon = dragIconRef;
    }

    public void UpdateSlot(ItemInstance item, bool isSelected)
    {
        currentItem = item;

        if (item == null)
        {
            itemImage.enabled = false;
            itemImage.sprite = null;
            quantityText.text = ""; // no item in slot
        }
        else
        {
            itemImage.enabled = true;
            itemImage.sprite = item.itemData.itemImage;

            Debug.Log("Showing icon: " +
                item.itemData.itemImage);

            currentItem = item;

            if (item.itemData.stackable && item.quantity > 1)
            {
                quantityText.text = "x" + item.quantity;
            }
            else
            {
                quantityText.text = "";
            }
        }

        //selectionBorder.SetActive(isSelected);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem == null) return;
        tooltip.ShowTooltip(currentItem.itemData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.HideTooltip();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem == null) return;
        tooltip.HideTooltip();
        dragIcon.sprite = currentItem.itemData.itemImage;
        dragIcon.gameObject.SetActive(true);
        dragIcon.transform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentItem == null) return;
        // icon follow mouse
        dragIcon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // hide when dragging finishes
        dragIcon.gameObject.SetActive(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        // swap when dropped into another slot
        InventorySlotUI draggedSlot = 
            eventData.pointerDrag?.GetComponent<InventorySlotUI>();

        if (draggedSlot == null) return;
        if (draggedSlot.SlotIndex == slotIndex) return;

        inventory.SwapItems(
            draggedSlot.SlotIndex, slotIndex);
    }
}
