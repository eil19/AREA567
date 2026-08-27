using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    [SerializeField]
    private Inventory inventory;

    private void Start()
    {
        if (inventory == null)
        {
            inventory =
                FindFirstObjectByType<Inventory>();
        }
    }

    public bool PickUp(ItemInstance item)
    {
        if (inventory == null)
        {
            Debug.LogError(
                "ItemPickUp could not find Inventory."
            );

            return false;
        }

        if (item == null ||
            item.itemData == null)
        {
            return false;
        }

        return inventory.AddItem(item);
    }
}