using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChestSlotUI :
    MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IDropHandler
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text quantityText;

    private int slotIndex;
    private ChestStorage chestStorage;
    private Inventory inventory;
    private Image dragIcon;

    private ItemInstance currentItem;

    public int SlotIndex => slotIndex;
    public ItemInstance CurrentItem =>
        currentItem;

    public void Initialise(
        int index,
        ChestStorage chestStorageRef,
        Inventory inventoryRef,
        Image dragIconRef)
    {
        slotIndex = index;
        chestStorage = chestStorageRef;
        inventory = inventoryRef;
        dragIcon = dragIconRef;
    }

    public void UpdateSlot(ItemInstance item)
    {
        currentItem = item;

        if (item == null)
        {
            itemImage.enabled = false;
            itemImage.sprite = null;
            quantityText.text = "";
            return;
        }

        itemImage.enabled = true;

        itemImage.sprite =
            item.itemData.itemImage;

        quantityText.text =
            item.quantity > 1
            ? "x" + item.quantity
            : "";
    }

    public void OnBeginDrag(
        PointerEventData eventData)
    {
        if (currentItem == null)
            return;

        dragIcon.sprite =
            currentItem.itemData.itemImage;

        dragIcon.gameObject.SetActive(true);

        dragIcon.transform.position =
            eventData.position;
    }

    public void OnDrag(
        PointerEventData eventData)
    {
        if (currentItem == null)
            return;

        dragIcon.transform.position =
            eventData.position;
    }

    public void OnEndDrag(
        PointerEventData eventData)
    {
        dragIcon.gameObject.SetActive(false);
    }

    public void OnDrop(
        PointerEventData eventData)
    {
        // Inventory -> Chest
        InventorySlotUI inventorySlot =
            eventData.pointerDrag?
            .GetComponent<InventorySlotUI>();

        if (inventorySlot != null)
        {
            ItemInstance item =
                inventorySlot.CurrentItem;

            if (item == null)
                return;

            bool added =
                chestStorage.AddItemAtSlot(
                    slotIndex,
                    item.itemData,
                    item.itemEffect,
                    item.quantity
                );

            if (!added)
                return;

            inventory.RemoveQuantityAtSlot(
                inventorySlot.SlotIndex,
                item.quantity
            );

            return;
        }

        // Chest -> Chest
        ChestSlotUI chestSlot =
            eventData.pointerDrag?
            .GetComponent<ChestSlotUI>();

        if (chestSlot != null)
        {
            chestStorage.SwapItems(
                chestSlot.SlotIndex,
                slotIndex
            );
        }
    }

    public bool ReturnToInventory(
        int inventorySlotIndex)
    {
        if (currentItem == null)
            return false;

        bool added =
            inventory.AddItemAtSlot(
                inventorySlotIndex,
                currentItem.itemData,
                currentItem.itemEffect,
                currentItem.quantity
            );

        if (!added)
            return false;

        chestStorage.RemoveQuantityAtSlot(
            slotIndex,
            currentItem.quantity
        );

        return true;
    }
}