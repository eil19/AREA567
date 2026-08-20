using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private InventorySlotUI[] slots;
    [SerializeField] private ItemTooltipUI tooltip;
    [SerializeField] private Image dragIcon;

    private void Start()
    {
        InitialiseSlots();
        RefreshInventory();
    }

    private void InitialiseSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].Initialise(i, tooltip, inventory, dragIcon);
        }
    }

    public void RefreshInventory()
    {
        if (inventory == null) return;

        for (int i = 0; i < slots.Length; i++)
        {
            ItemInstance item = inventory.GetItem(i);
            bool isSelected = inventory.SelectedSlotIndex == i;
            slots[i].UpdateSlot(item, isSelected);
        }
    }
}