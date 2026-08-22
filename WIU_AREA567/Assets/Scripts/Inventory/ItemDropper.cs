using UnityEngine;
using UnityEngine.Events;

public class ItemDropper : MonoBehaviour
{
    [SerializeField] private Inventory inventory;

    [Header("Drop")]
    [SerializeField] private Item worldItemPrefab;
    [SerializeField] private Transform dropPoint;

    [Header("Events")]
    public UnityEvent<ItemData, int> OnItemDropped;

    [SerializeField] private float dropOffset = 1f;

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
    }

    public bool DropOne(int slotIndex)
    {
        ItemInstance item = inventory.GetItem(slotIndex);
        if (item == null) return false;
        ItemData itemData = item.itemData;
        SpawnDroppedItem(itemData, 1);
        return inventory.RemoveQuantityAtSlot(slotIndex, 1);
    }

    public bool DropStack(int slotIndex)
    {
        ItemInstance item = inventory.GetItem(slotIndex);
        if (item == null) return false;
        ItemData itemData = item.itemData;
        int quantity = item.quantity;
        SpawnDroppedItem(itemData, quantity);
        return inventory.RemoveQuantityAtSlot(slotIndex, quantity);
    }

    private void SpawnDroppedItem(ItemData itemData, int quantity)
    {
        Vector3 spawnPosition = dropPoint != null ?
            dropPoint.position : transform.position + transform.right * dropOffset;
        Item droppedItem = Instantiate(
            worldItemPrefab, spawnPosition, Quaternion.identity);
        droppedItem.Initialise(itemData, quantity);
        OnItemDropped?.Invoke(itemData, quantity);
    }
}