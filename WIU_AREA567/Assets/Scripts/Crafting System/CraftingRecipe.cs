using UnityEngine;

[CreateAssetMenu(fileName = "CraftingRecipe", menuName = "CraftingSystem/CraftingRecipe")]
public class CraftingRecipe : ScriptableObject
{
    [Header("Recipe Information")]
    public string recipeName;

    [TextArea]
    public string recipeDescription;

    [Header("Recipe Display")]
    public Sprite formulaImage;

    [Header("3x3 grid")]
    public CraftingIngredient[] recipeGrid = new CraftingIngredient[9];

    [Header("Output")]
    public ItemData outputItem;
    public ItemEffect outputEffect;
    public int outputQuantity = 1;
}
