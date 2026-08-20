using UnityEngine;

[CreateAssetMenu(fileName = "CraftingRecipe", menuName = "CraftingSystem/CraftingRecipe")]
public class CraftingRecipe : ScriptableObject
{
    public Item outputItem; // result item of grid
    public Item[] recipeGrid = new Item[9]; // 3x3 grid
}
