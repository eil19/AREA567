using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    [SerializeField] private Inventory inventory;

    private void Start()
    {
        if (inventory == null)
        {
            inventory = FindFirstObjectByType<Inventory>();
        }
    }

    public bool PickUp(ItemInstance item)
    {
        return inventory.AddItem(item);
    }
}
