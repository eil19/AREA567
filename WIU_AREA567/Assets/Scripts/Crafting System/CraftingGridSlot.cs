[System.Serializable]

public class CraftingGridSlot
{
    public ItemData itemData;
    public ItemEffect itemEffect;
    public int quantity;

    public bool IsEmpty => itemData == null || quantity <= 0;

    public void SetItem(ItemData item, ItemEffect effect, int amount)
    {
        itemData = item;
        itemEffect = effect;
        quantity = amount;
    }

    public void Clear()
    {
        itemData = null;
        itemEffect = null;
        quantity = 0;
    }
}