using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDetailsUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text descriptionText;

    [SerializeField] private Button useButton;
    [SerializeField] private Button backButton;

    private Inventory inventory;
    private ItemDropper itemDropper;
    private GameObject player;

    private int selectedSlotIndex = -1;

    public void Initialise(Inventory inventoryRef, ItemDropper itemDropperRef, GameObject playerRef)
    {
        inventory = inventoryRef;
        itemDropper = itemDropperRef;
        player = playerRef;

        Hide();
    }

    public void ShowForSlot(int slotIndex)
    {
        ItemInstance item = inventory.GetItem(slotIndex);

        if (item == null)
        {
            Hide();
            return;
        }

        selectedSlotIndex = slotIndex;

        itemNameText.text = item.itemData.itemName;
        descriptionText.text = item.itemData.description;

        // materials cannot be used by player
        bool canUse = item.itemData.itemType != ItemType.Material;

        useButton.gameObject.SetActive(canUse);
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        selectedSlotIndex = -1;
        gameObject.SetActive(false);
    }

    public void UseSelectedItem()
    {
        if (selectedSlotIndex < 0) return;

        ItemInstance item = inventory.GetItem(selectedSlotIndex);

        if (item == null || item.itemEffect == null) return;

        item.itemEffect.Use(player);
        inventory.TryConsumeItem(item.itemData, 1);

        Refresh();
    }

    public void DropOne()
    {
        if (selectedSlotIndex < 0) return;

        itemDropper.DropOne(selectedSlotIndex);

        Refresh();
    }

    public void DropAll()
    {
        if (selectedSlotIndex < 0) return;

        itemDropper.DropStack(selectedSlotIndex);

        Hide();
    }

    private void Refresh()
    {
        if (selectedSlotIndex < 0) return;

        ItemInstance item = inventory.GetItem(selectedSlotIndex);

        if (item == null)
        {
            Hide();
            return;
        }

        ShowForSlot(selectedSlotIndex);
    }
}