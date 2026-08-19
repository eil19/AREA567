using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    [SerializeField] private Inventory inventory;

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

    public bool PickUp(ItemInstance item)
    {
        return inventory.AddItem(item);
    }
}
