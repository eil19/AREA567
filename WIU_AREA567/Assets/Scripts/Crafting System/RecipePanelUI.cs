using System.Text;
using TMPro;
using UnityEngine;

public class RecipePanelUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ResearchLog researchLog;

    [Header("Recipe Details")]
    [SerializeField] private GameObject recipeDetailsPanel;
    [SerializeField] private TMP_Text recipeNameText;
    [SerializeField] private TMP_Text recipeDescriptionText;
    [SerializeField] private TMP_Text ingredientsText;

    public void ShowRecipe(CraftingRecipe recipe)
    {
        if (recipe == null)
        {
            Debug.LogWarning("No CraftingRecipe assigned.");
            return;
        }

        recipeDetailsPanel.SetActive(true);

        // Recipe has not been discovered yet
        if (researchLog == null ||
            !researchLog.IsRecipeUnlocked(recipe))
        {
            ShowLockedRecipe(recipe);
            return;
        }

        // Recipe has been discovered
        ShowUnlockedRecipe(recipe);
    }

    private void ShowLockedRecipe(CraftingRecipe recipe)
    {
        recipeNameText.text = recipe.recipeName;

        recipeDescriptionText.text =
            "Recipe has not been unlocked yet.";

        ingredientsText.text =
            "Find research notes to unlock this recipe.";
    }

    private void ShowUnlockedRecipe(CraftingRecipe recipe)
    {
        recipeNameText.text = recipe.recipeName;
        recipeDescriptionText.text = recipe.recipeDescription;

        ingredientsText.text = GetIngredientsText(recipe);
    }

    private string GetIngredientsText(CraftingRecipe recipe)
    {
        StringBuilder text = new StringBuilder();

        text.AppendLine("Required Materials:");

        foreach (CraftingIngredient ingredient in recipe.recipeGrid)
        {
            if (ingredient == null ||
                ingredient.item == null ||
                ingredient.quantity <= 0)
            {
                continue;
            }

            text.AppendLine(
                ingredient.item.itemName +
                " x" +
                ingredient.quantity
            );
        }

        return text.ToString();
    }

    public void HideRecipeDetails()
    {
        recipeDetailsPanel.SetActive(false);
    }
}