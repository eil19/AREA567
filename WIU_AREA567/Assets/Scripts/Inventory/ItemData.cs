using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea]
    public string description;
    public Sprite itemImage;
    public bool stackable;
}
