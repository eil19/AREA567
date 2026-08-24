using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private int maxItems = 8;

    [Header("Events")]
    public UnityEvent OnInventoryChanged;
    public UnityEvent<ItemData, int> OnItemAdded;
    public UnityEvent<ItemData, int> OnItemConsumed;

    private List<ItemInstance> items = new List<ItemInstance>();
    private int selectedSlotIndex = 0;

    public IReadOnlyList<ItemInstance> Items => items;
    public int MaxItems => maxItems;
    public int SelectedSlotIndex => selectedSlotIndex;

    private void Awake()
    {
        InitialiseInventory();
    }

    private void InitialiseInventory()
    {
        items.Clear();
        for (int i = 0; i < maxItems; i++)
        {
            items.Add(null);
        }
    }

    public bool AddItem(ItemInstance newItem)
    {
        if (newItem == null || newItem.itemData == null || newItem.quantity <= 0) return false;

        ItemData itemData = newItem.itemData;
        int quantityToAdd = newItem.quantity;

        if (!CanAddItem(itemData, quantityToAdd))
        {
            Debug.Log("Inventory does not have enough space.");
            return false;
        }

        int remaining = quantityToAdd;

        // try to stack item first
        if (itemData.stackable)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (!CanPlaceItemInSlot(i, itemData)) continue;

                ItemInstance currentItem = items[i];
                if (currentItem == null) continue;
                if (currentItem.itemData != itemData) continue;
                if (currentItem.quantity >= itemData.maxStack) continue;

                int availableSpace = itemData.maxStack - currentItem.quantity;
                int amountToAdd = Mathf.Min(availableSpace, remaining);

                currentItem.quantity += amountToAdd;
                remaining -= amountToAdd;

                if (remaining <= 0) break;
            }
        }

        // put remaining quantity into empty slots
        while (remaining > 0)
        {
            int emptyIndex = FindEmptySlot(itemData);
            if (emptyIndex < 0) return false;

            int amountForSlot;
            if (itemData.stackable)
            {
                amountForSlot = Mathf.Min(itemData.maxStack, remaining);
            }
            else
            {
                amountForSlot = 1;
            }

            items[emptyIndex] = new ItemInstance(itemData, newItem.itemEffect, amountForSlot);
            remaining -= amountForSlot;
        }

        OnInventoryChanged?.Invoke();
        OnItemAdded?.Invoke(itemData, quantityToAdd);

        return true;
    }

    private int FindEmptySlot(ItemData itemData)
    {
        if (itemData == null) return -1;

        for (int i = 0; i < items.Count; i++)
        {
            if (!CanPlaceItemInSlot(i, itemData)) continue;
            if (items[i] == null) return i;
        }
        return -1;
    }

    public bool CanAddItem(ItemData itemData, int quantity)
    {
        if (itemData == null || quantity <= 0) return false;

        int availableCapacity = 0;

        for (int i = 0; i < items.Count; i++)
        {
            if (!CanPlaceItemInSlot(i, itemData)) continue;

            ItemInstance currentItem = items[i];
            // empty valid slot
            if (currentItem == null)
            {
                availableCapacity += itemData.stackable? itemData.maxStack : 1;
                continue;
            }
            // existing matching stack
            if (itemData.stackable && currentItem.itemData == itemData)
            {
                availableCapacity += itemData.maxStack - currentItem.quantity;
            }
        }
        return availableCapacity >= quantity;
    }

    public ItemInstance GetItem(int index)
    {
        if (index < 0 || index >= items.Count) return null;
        return items[index];
    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index >= items.Count) return;

        items[index] = null;
        OnInventoryChanged?.Invoke();
    }

    public bool SwapItems(int firstIndex, int secondIndex)
    {
        if (firstIndex < 0 ||
            firstIndex >= items.Count ||
            secondIndex < 0 ||
            secondIndex >= items.Count) return false;

        if (firstIndex == secondIndex) return false;

        ItemInstance firstItem = items[firstIndex];
        ItemInstance secondItem = items[secondIndex];

        // can first item move to second slot?
        if (firstItem != null && !CanPlaceItemInSlot(secondIndex, firstItem.itemData)) return false;

        // can second item move to first slot
        if (secondItem != null && !CanPlaceItemInSlot(firstIndex, secondItem.itemData)) return false;

        items[firstIndex] = items[secondIndex];
        items[secondIndex] = firstItem;
        OnInventoryChanged?.Invoke();
        return true;
    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= items.Count) return;
        selectedSlotIndex = index;
        OnInventoryChanged?.Invoke();
    }

    public void ClearInventory()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i] = null;
        }
        OnInventoryChanged?.Invoke();
    }

    public void DisplayItems()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                Debug.Log("Slot " + i + ": Empty");
            }
            else
            {
                Debug.Log(
                    "Slot " + i + ": " + items[i].itemData.itemName
                    + " x" + items[i].quantity
                    );
            }
        }
    }

    public int GetItemQuantity(ItemData itemData)
    {
        int totalQuantity = 0;

        for (int i = 0; i < items.Count; i++)
        {
            ItemInstance item = items[i];

            if (item == null)
                continue;

            if (item.itemData == itemData)
            {
                totalQuantity += item.quantity;
            }
        }

        return totalQuantity;
    }

    public bool RemoveQuantityAtSlot(int index, int quantity)
    {
        if (index < 0 || index >= items.Count) return false;

        ItemInstance item = items[index];
        if (item == null || quantity <= 0 || item.quantity < quantity) return false;

        item.quantity -= quantity;
        if (item.quantity <= 0)
        {
            items[index] = null;
        }
        OnInventoryChanged?.Invoke();
        return true;
    }

    // overload for returning something from crafting grid into inventory
    public bool AddItem(ItemData itemData, int quantity)
    {
        ItemInstance item = new ItemInstance(itemData, null, quantity);
        return AddItem(item);
    }

    public bool AddItemAtSlot(int index, ItemData itemData, 
        ItemEffect itemEffect, int quantity)
    {
        if (index < 0 || index >= items.Count || itemData == null || quantity <= 0) return false;
        if (!CanPlaceItemInSlot(index, itemData)) return false;

        ItemInstance currentItem = items[index];

        // empty slot
        if (currentItem == null)
        {
            if (itemData.stackable && quantity > itemData.maxStack) return false;
            if (!itemData.stackable && quantity > 1) return false;

            items[index] = new ItemInstance(itemData, itemEffect, quantity);
            OnInventoryChanged?.Invoke();
            return true;
        }

        // cannot merge different items
        if (currentItem.itemData != itemData) return false;
        // cannot merge non-stackable items
        if (!itemData.stackable) return false;

        if (currentItem.quantity + quantity > itemData.maxStack) return false;

        currentItem.quantity += quantity;
        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool HasItem(ItemData itemData, int quantity = 1)
    {
        // check whether enough of an item exists
        if (itemData == null || quantity <= 0) return false;
        return GetItemQuantity(itemData) >= quantity;
    }

    // consume item by type
    public bool TryConsumeItem(ItemData itemData, int quantity = 1)
    {
        if (itemData == null || quantity <= 0) return false;
        if (!HasItem(itemData, quantity)) return false;

        int remaining = quantity;

        for (int i = 0; i < items.Count; i++)
        {
            ItemInstance item = items[i];
            if (item == null) continue;
            if (item.itemData != itemData) continue;

            int amountToRemove = Mathf.Min(item.quantity, remaining);
            item.quantity -= amountToRemove;
            remaining -= amountToRemove;
            if (item.quantity <= 0)
            {
                items[i] = null;
            }
            if (remaining <= 0) break;
        }

        OnInventoryChanged?.Invoke();
        OnItemConsumed?.Invoke(itemData, quantity);

        return true;
    }

    public bool CanPlaceItemInSlot(int slotIndex, ItemData itemData)
    {
        if (itemData == null) return false;
        if (slotIndex < 0 || slotIndex >= items.Count) return false;

        // slots 0-2 reserved for weapons
        if (slotIndex <= 2)
        {
            return itemData.itemType == ItemType.Weapon;
        }

        // slots 3-7 are for normal inventory items
        return itemData.itemType != ItemType.Weapon;
    }
}