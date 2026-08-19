using UnityEngine;

[CreateAssetMenu(fileName = "ItemEffect", menuName = "Inventory/ItemEffect")]
public abstract class ItemEffect : ScriptableObject
{
    public abstract void Use(GameObject user);
}
