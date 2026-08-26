using UnityEngine;

public class RecipeButtonUI : MonoBehaviour
{
    [SerializeField] private CraftingRecipe recipe;
    [SerializeField] private RecipePanelUI recipePanelUI;

    public void ShowRecipe()
    {
        if (recipePanelUI == null || recipe == null) return;

        recipePanelUI.ShowRecipe(recipe);
    }
}