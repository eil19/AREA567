using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour,
    IPointerClickHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IDropHandler
{
    [Header("UI References")]
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text quantityText;

    private int slotIndex;
    public int SlotIndex => slotIndex;

    private ItemInstance currentItem;
    public ItemInstance CurrentItem => currentItem;
    private ItemTooltipUI tooltipUI;
    private Inventory inventory;
    private Image dragIcon;

    public void Initialise(int index, Inventory inventoryRef, Image dragIconRef, 
        ItemTooltipUI tooltipRef)
    {
        slotIndex = index;
        inventory = inventoryRef;
        dragIcon = dragIconRef;
        tooltipUI = tooltipRef;
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
            if (item.itemData.stackable && item.quantity > 1)
            {
                quantityText.text = "x" + item.quantity;
            }
            else
            {
                quantityText.text = "";
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem == null) return;
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
        // inventory -> inventory
        InventorySlotUI draggedInventorySlot = eventData.pointerDrag?.GetComponent<InventorySlotUI>();

        if (draggedInventorySlot != null)
        {
            if (draggedInventorySlot.SlotIndex == slotIndex) return;
            inventory.SwapItems(draggedInventorySlot.SlotIndex, slotIndex);
            return;
        }

        // crafting -> inventory
        CraftingSlotUI draggedCraftingSlot = eventData.pointerDrag?.GetComponent<CraftingSlotUI>();

        if (draggedCraftingSlot != null)
        {
            draggedCraftingSlot.ReturnOneToInventory(slotIndex);
            return;
        }

        // crafting output -> inventory
        CraftingOutputUI draggedOutput = eventData.pointerDrag?.GetComponent<CraftingOutputUI>();
        if (draggedOutput != null)
        {
            draggedOutput.CraftingSystem.CraftCurrentRecipeToSlot(slotIndex);
            return;
        }

        // chest -> inventory
        ChestSlotUI draggedChestSlot = eventData.pointerDrag?.GetComponent<ChestSlotUI>();

        if (draggedChestSlot != null)
        {
            draggedChestSlot.ReturnToInventory(slotIndex);
            return;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentItem == null) return;

        // weapon slots reserved for future weapon logic
        if (SlotIndex <= 2) return;

        if (tooltipUI != null)
        {
            tooltipUI.ShowForSlot(slotIndex);
        }
    }
}