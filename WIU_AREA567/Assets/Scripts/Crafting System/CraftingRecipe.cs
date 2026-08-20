using UnityEngine;

[CreateAssetMenu(fileName = "CraftingRecipe", menuName = "CraftingSystem/CraftingRecipe")]
public class CraftingRecipe : ScriptableObject
{
    [Header("Recipe Information")]
    public string recipeName;

    [TextArea]
    public string recipeDescription;

    [Header("Ingredients")]
    public CraftingIngredient[] ingredients;

    [Header("Output")]
    public ItemData outputItem;
    public int outputQuantity = 1;
}
