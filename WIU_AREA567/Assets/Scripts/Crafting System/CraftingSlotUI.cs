using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingSlotUI : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IDropHandler
{
    [Header("UI References")]
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text quantityText;

    private int slotIndex;
    private CraftingSystem craftingSystem;
    private Inventory inventory;
    private Image dragIcon;

    private CraftingGridSlot currentSlot;
    public int SlotIndex => slotIndex;
    public CraftingGridSlot CurrentSlot => currentSlot;

    public void Initialise(int index, CraftingSystem craftingSystemRef, Inventory inventoryRef, Image dragIconRef)
    {
        slotIndex = index;
        craftingSystem = craftingSystemRef;
        inventory = inventoryRef;
        dragIcon = dragIconRef;
    }

    public void UpdateSlot(CraftingGridSlot slot)
    {
        currentSlot = slot;

        if (slot == null || slot.IsEmpty)
        {
            itemImage.enabled = false;
            itemImage.sprite = null;
            quantityText.text = "";
            return;
        }

        itemImage.enabled = true;
        itemImage.sprite = slot.itemData.itemImage;

        if (slot.quantity > 1)
        {
            quantityText.text = "x" + slot.quantity;
        }
        else
        {
            quantityText.text = "";
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentSlot == null || currentSlot.IsEmpty || dragIcon == null) return;

        dragIcon.sprite = currentSlot.itemData.itemImage;
        dragIcon.gameObject.SetActive(true);
        dragIcon.transform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentSlot == null || currentSlot.IsEmpty || dragIcon == null) return;
        dragIcon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
        {
            dragIcon.gameObject.SetActive(false);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        // inventory -> crafting
        InventorySlotUI inventorySlot = 
            eventData.pointerDrag?.GetComponent<InventorySlotUI>();

        if (inventorySlot != null)
        {
            TransferFromInventory(inventorySlot);
            return;
        }

        // crafting
        CraftingSlotUI craftingSlot = 
            eventData.pointerDrag?.GetComponent<CraftingSlotUI>();

        if (craftingSlot != null)
        {
            TransferFromCrafting(craftingSlot);
        }
    }

    private void TransferFromInventory(InventorySlotUI source)
    {
        ItemInstance item = source.CurrentItem;

        if (item == null) return;

        bool placed = craftingSystem.PlaceItem(
            slotIndex, item.itemData, item.itemEffect, 1);

        if (!placed) return;

        bool removed = inventory.RemoveQuantityAtSlot(
            source.SlotIndex, 1);

        // rollback if inventory removal failed
        if (!removed)
        {
            craftingSystem.RemoveFromGrid(
                slotIndex, 1);
        }
    }

    private void TransferFromCrafting(CraftingSlotUI source)
    {
        if (source.SlotIndex == slotIndex) return;

        CraftingGridSlot sourceItem = source.CurrentSlot;

        if (sourceItem == null || sourceItem.IsEmpty) return;

        bool placed = craftingSystem.PlaceItem(
            slotIndex, sourceItem.itemData, sourceItem.itemEffect, 1);
        if (!placed) return;

        craftingSystem.RemoveFromGrid(
            source.SlotIndex, 1);
    }

    public bool ReturnOneToInventory(int targetInventorySlot)
    {
        if (currentSlot == null || currentSlot.IsEmpty) return false;

        bool added = inventory.AddItemAtSlot(
            targetInventorySlot, currentSlot.itemData, 
            currentSlot.itemEffect,
            1);

        if (!added) return false;
        craftingSystem.RemoveFromGrid(slotIndex, 1);
        return true;
    }
}
