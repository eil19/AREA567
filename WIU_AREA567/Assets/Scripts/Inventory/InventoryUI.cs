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
        if (inventory == null)
        {
            GameObject inventoryObject = GameObject.Find("Inventory");
            if (inventoryObject != null)
            {
                inventory = inventoryObject.GetComponent<Inventory>();
            }
        }

        if (inventory != null)
        {
            inventory.OnInventoryChanged.AddListener(RefreshInventory);
        }
        InitialiseSlots();
        RefreshInventory();
    }

    private void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged.RemoveListener(RefreshInventory);
        }
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