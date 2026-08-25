using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipePanelUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ResearchLog researchLog;

    [Header("Recipe Details")]
    [SerializeField] private GameObject recipeDetailsPanel;
    [SerializeField] private TMP_Text recipeNameText;
    [SerializeField] private Image formulaImage;
    [SerializeField] private TMP_Text lockedText;

    private void Start()
    {
        if (researchLog == null)
        {
            researchLog = FindFirstObjectByType<ResearchLog>();
        }

        if (recipeDetailsPanel != null)
        {
            recipeDetailsPanel.SetActive(false);
        }
    }

    public void ShowRecipe(CraftingRecipe recipe)
    {
        if (recipe == null)
        {
            Debug.Log("No CraftingRecipe assigned.");
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
        // hide formula
        formulaImage.sprite = null;
        formulaImage.gameObject.SetActive(false);
        lockedText.text = "Recipe has not been unlocked yet.";
        lockedText.gameObject.SetActive(true);
    }

    private void ShowUnlockedRecipe(CraftingRecipe recipe)
    {
        recipeNameText.text = recipe.recipeName;

        // hide locked message
        lockedText.gameObject.SetActive(false);

        // show crafting formula
        if (recipe.formulaImage != null)
        {
            formulaImage.sprite = recipe.formulaImage;
            formulaImage.gameObject.SetActive(true);
        }
        else
        {
            formulaImage.sprite = null;
            formulaImage.gameObject.SetActive(false);
            Debug.Log("No formula image assigned to recipe: " + recipe.recipeName);
        }
    }

    public void HideRecipeDetails()
    {
        recipeDetailsPanel.SetActive(false);
    }
}