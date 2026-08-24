using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChestStorage : MonoBehaviour
{
    [Header("Chest Settings")]
    [SerializeField]
    private string chestId = "LaboratoryChest";

    [SerializeField]
    private int maxSlots = 5;

    [Header("Events")]
    public UnityEvent OnStorageChanged;

    private ChestStorageService storageService;

    private List<ItemInstance> items;

    public int MaxSlots => maxSlots;

    private void Start()
    {
        storageService =
            FindFirstObjectByType<
                ChestStorageService>();

        if (storageService == null)
        {
            Debug.LogError(
                "ChestStorage could not find " +
                "ChestStorageService."
            );

            return;
        }

        items =
            storageService.GetOrCreateChest(
                chestId,
                maxSlots
            );

        OnStorageChanged?.Invoke();
    }

    public ItemInstance GetItem(int index)
    {
        if (items == null ||
            index < 0 ||
            index >= items.Count)
        {
            return null;
        }

        return items[index];
    }

    public bool CanStore(
        ItemData itemData)
    {
        if (itemData == null)
            return false;

        return
            itemData.itemType ==
                ItemType.Material
            ||
            itemData.itemType ==
                ItemType.Consumable;
    }

    public bool AddItemAtSlot(
        int index,
        ItemData itemData,
        ItemEffect itemEffect,
        int quantity)
    {
        if (items == null ||
            index < 0 ||
            index >= items.Count ||
            itemData == null ||
            quantity <= 0)
        {
            return false;
        }

        if (!CanStore(itemData))
            return false;

        ItemInstance current =
            items[index];

        if (current == null)
        {
            if (itemData.stackable &&
                quantity > itemData.maxStack)
            {
                return false;
            }

            if (!itemData.stackable &&
                quantity > 1)
            {
                return false;
            }

            items[index] =
                new ItemInstance(
                    itemData,
                    itemEffect,
                    quantity
                );

            OnStorageChanged?.Invoke();

            return true;
        }

        if (current.itemData != itemData)
            return false;

        if (!itemData.stackable)
            return false;

        if (current.quantity + quantity >
            itemData.maxStack)
        {
            return false;
        }

        current.quantity += quantity;

        OnStorageChanged?.Invoke();

        return true;
    }

    public bool RemoveQuantityAtSlot(
        int index,
        int quantity)
    {
        if (items == null ||
            index < 0 ||
            index >= items.Count)
        {
            return false;
        }

        ItemInstance item =
            items[index];

        if (item == null ||
            quantity <= 0 ||
            item.quantity < quantity)
        {
            return false;
        }

        item.quantity -= quantity;

        if (item.quantity <= 0)
        {
            items[index] = null;
        }

        OnStorageChanged?.Invoke();

        return true;
    }

    public bool SwapItems(
        int firstIndex,
        int secondIndex)
    {
        if (items == null ||
            firstIndex < 0 ||
            firstIndex >= items.Count ||
            secondIndex < 0 ||
            secondIndex >= items.Count ||
            firstIndex == secondIndex)
        {
            return false;
        }

        ItemInstance temp =
            items[firstIndex];

        items[firstIndex] =
            items[secondIndex];

        items[secondIndex] =
            temp;

        OnStorageChanged?.Invoke();

        return true;
    }
}