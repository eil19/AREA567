using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemInstance item;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite =
            item.itemData.itemImage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out ItemPickUp itemPickUp))
        {
            bool pickedUp = itemPickUp.PickUp(item);
            if (pickedUp)
            {
                Destroy(gameObject);
            }
        }
    }
}