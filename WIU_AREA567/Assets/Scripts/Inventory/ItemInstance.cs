[System.Serializable]
public class ItemInstance
{
    public ItemData itemData;
    public ItemEffect itemEffect;
    public int quantity;

    public ItemInstance(ItemData itemData, ItemEffect itemEffect,
        int quantity = 1)
    {
        this.itemData = itemData;
        this.itemEffect = itemEffect;
        this.quantity = quantity;
    }
}