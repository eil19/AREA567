using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private InventorySlotUI[] slots;
    [SerializeField] private Image dragIcon;

    [SerializeField] private ItemTooltipUI tooltipUI;
    [SerializeField] private GameObject player;

    private IEnumerator Start()
    {
        // Allow persistent objects and scene player
        // to finish appearing after scene load.
        yield return null;

        while (inventory == null)
        {
            inventory =
                FindFirstObjectByType<Inventory>();

            if (inventory == null)
            {
                yield return null;
            }
        }

        while (player == null)
        {
            player =
                GameObject.FindGameObjectWithTag(
                    "Player"
                );

            if (player == null)
            {
                yield return null;
            }
        }

        InitialiseSlots();

        inventory.OnInventoryChanged
            .AddListener(RefreshInventory);

        RefreshInventory();
    }

    private void InitialiseSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                Debug.LogError(
                    "InventoryUI slot " +
                    i +
                    " is missing."
                );

                continue;
            }

            slots[i].Initialise(
                i,
                inventory,
                dragIcon,
                tooltipUI
            );
        }

        if (tooltipUI != null)
        {
            tooltipUI.Initialise(
                inventory,
                player
            );
        }
    }

    public void RefreshInventory()
    {
        if (inventory == null)
            return;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
                continue;

            ItemInstance item =
                inventory.GetItem(i);

            bool selected =
                inventory.SelectedSlotIndex == i;

            slots[i].UpdateSlot(
                item,
                selected
            );
        }
    }

    private void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged
                .RemoveListener(
                    RefreshInventory
                );
        }
    }
}