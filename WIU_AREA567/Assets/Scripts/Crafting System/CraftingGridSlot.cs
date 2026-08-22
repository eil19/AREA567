[System.Serializable]

public class CraftingGridSlot
{
    public ItemData itemData;
    public int quantity;

    public bool IsEmpty => itemData == null || quantity <= 0;

    public void SetItem(ItemData item, int amount)
    {
        itemData = item;
        quantity = amount;
    }

    public void Clear()
    {
        itemData = null;
        quantity = 0;
    }
}