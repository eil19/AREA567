using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    public List<ItemInstance> items = new List<ItemInstance>();
    public int maxItems = 10;

    public UnityEvent OnInventoryChanged;
    public IReadOnlyList<ItemInstance> Items => items;
    public int MaxItems => maxItems;
    private static Inventory existingInstance;

    private void Awake()
    {
        if (existingInstance != null && existingInstance != this)
        {
            Destroy(gameObject);
            return;
        }

        existingInstance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool AddItem(ItemInstance item)
    {
        if (item == null) return false;

        if (items.Count >= maxItems)
            return false;

        items.Add(item);
        OnInventoryChanged?.Invoke();
        return true;
    }

    public ItemInstance GetItem(int index)
    {
        if (index < 0 || index >= items.Count) { return null; }
        return items[index];
    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index >= items.Count) return;
        items.RemoveAt(index);
        OnInventoryChanged?.Invoke();
    }

    public void DisplayItems()
    {
        foreach (ItemInstance item in items)
        {
            Debug.Log("Item Name: " + item.itemData.itemName);
        }
    }

    public void ClearInventory()
    {
        items.Clear();
        OnInventoryChanged?.Invoke();
    }
}