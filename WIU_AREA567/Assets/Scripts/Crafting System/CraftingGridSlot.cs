[System.Serializable]

public class CraftingGridSlot
{
    public ItemData ItemData;
    public int quantity;

    public bool IsEmpty => ItemData == null || quantity <= 0;

    public void SetItem(ItemData item, int amount)
    {
        ItemData = item;
        quantity = amount;
    }

    public void Clear()
    {
        ItemData = null;
        quantity = 0;
    }
}