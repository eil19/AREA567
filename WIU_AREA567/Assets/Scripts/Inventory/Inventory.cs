using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private int maxItems = 5;
    [Header("Events")]
    public UnityEvent OnInventoryChanged;

    private List<ItemInstance> items = new List<ItemInstance>();
    private int selectedSlotIndex = 0;

    public IReadOnlyList<ItemInstance> Items => items;
    public int MaxItems => maxItems;
    public int SelectedSlotIndex => selectedSlotIndex;
    private static Inventory existingInstance;

    private void Awake()
    {
        InitialiseInventory();
        if (existingInstance != null && existingInstance != this)
        {
            Destroy(gameObject);
            return;
        }

        existingInstance = this;
        DontDestroyOnLoad(gameObject);
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
        if (newItem == null || newItem.itemData == null) return false;

        // try to stack item first
        if (newItem.itemData.stackable)
        {
            for (int i = 0; i < items.Count; i++)
            {
                ItemInstance currentItem = items[i];
                if (currentItem == null) continue;
                if (currentItem.itemData != newItem.itemData) continue;
                if (currentItem.quantity >= currentItem.itemData.maxStack) continue;

                int availableSpace = currentItem.itemData.maxStack - currentItem.quantity;
                int amountToAdd = Mathf.Min(availableSpace, newItem.quantity);

                currentItem.quantity += amountToAdd;
                newItem.quantity -= amountToAdd;

                if (newItem.quantity <= 0)
                {
                    OnInventoryChanged?.Invoke();
                    return true;
                }
            }
        }

        // find empty inventory slot
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                items[i] = newItem;
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        Debug.Log("Inventory is full");
        return false;
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

    public void SwapItems(int firstIndex, int secondIndex)
    {
        if (firstIndex < 0 ||
            firstIndex >= items.Count ||
            secondIndex < 0 ||
            secondIndex >= items.Count) return;

        ItemInstance tempItem = items[firstIndex];
        items[firstIndex] = items[secondIndex];
        items[secondIndex] = tempItem;

        OnInventoryChanged?.Invoke();
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
}