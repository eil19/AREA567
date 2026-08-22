using UnityEngine;

public class Item : MonoBehaviour 
    , IPickupable
{
    [SerializeField] private ItemInstance item;

    private void Start()
    {
        if (item != null && 
            item.itemData != null)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = item.itemData.itemImage;
            }
        }
    }

    public void Initialise(ItemData itemData, int quantity)
    {
        item = new ItemInstance(itemData, null, quantity);
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && itemData != null)
        {
            spriteRenderer.sprite = itemData.itemImage;
        }
    }

    public void Pickup(GameObject picker)
    {
        Debug.Log("Item.Pickup()  called for " + gameObject.name);
        if (picker.TryGetComponent(out ItemPickUp itemPickUp))
        {
            bool pickedUp = itemPickUp.PickUp(item);
            if (pickedUp)
            {
                Debug.Log("Picked up: " + item.itemData.itemName);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Could not pick up " + item.itemData.itemName +
                    ". Inventory is full");
            }
        }
    }
}