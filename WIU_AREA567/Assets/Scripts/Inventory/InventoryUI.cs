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

    private void Start()
    {
        if (inventory == null)
        {
            inventory = FindFirstObjectByType<Inventory>();
        }
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
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
            slots[i].Initialise(i, inventory, dragIcon, tooltipUI);
        }

        if (tooltipUI != null)
        {
            tooltipUI.Initialise(inventory, player);
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