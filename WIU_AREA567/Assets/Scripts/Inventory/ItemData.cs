using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea]
    public string description;
    public Sprite itemImage;
    public bool stackable;
    public int maxStack = 99;
    public ItemType itemType;

    [Header("Weapon")]
    public WeaponType weaponType;
}

public enum ItemType
{
    Material,
    Consumable,
    Weapon
}