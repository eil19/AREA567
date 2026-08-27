using System.Collections.Generic;
using TMPro;
using UnityEngine;

// REFERENCE STUB for Sze Yee's Crafting Bench UI - shows a plain text list
// of currently-unlocked recipe names. Swap for real icon-grid UI later;
// this just proves the unlock data flow works. Subscribes via code, not
// Inspector wiring, since ResearchLog is a persistent DontDestroyOnLoad
// singleton - Inspector-wired listeners would go stale on scene reload.
public class RecipeUnlockPanelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text listText;
    [SerializeField] private List<CraftingRecipe> allPossibleRecipes; // Sze Yee's recipe assets

    void Start()
    {
        ResearchLog.Instance.OnResearchLogChanged.AddListener(Refresh);
        Refresh();
    }

    void OnDestroy()
    {
        if (ResearchLog.Instance != null)
            ResearchLog.Instance.OnResearchLogChanged.RemoveListener(Refresh);
    }

    private void Refresh()
    {
        listText.text = "";
        foreach (CraftingRecipe recipe in allPossibleRecipes)
        {
            if (ResearchLog.Instance.IsRecipeUnlocked(recipe))
            {
                listText.text += recipe.recipeName + "\n";
            }
        }
    }
}