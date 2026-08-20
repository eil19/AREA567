using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CraftingSystem : MonoBehaviour
{
    public const int GRID_SIZE = 3;
    public UnityEvent OnGridChanged;

    private List<CraftingRecipe> craftingRecipesList;

    // check crafting output
    public Item CheckCraftingOutput(Item[] gridItems)
    {
        foreach (var recipe in craftingRecipesList)
        {
            if (IsMatchingRecipe(gridItems, recipe.recipeGrid))
            {
                return recipe.outputItem; // return output if recipe matches
            }
        }
        return null; // no matching recipe
    }

    private bool IsMatchingRecipe(Item[] grid, Item[] recipe)
    {
        for (int i = 0; i < grid.Length; i++)
        {
            if (grid[i] != recipe[i])
            {
                return false;
            }
        }
        return true;
    }
}
