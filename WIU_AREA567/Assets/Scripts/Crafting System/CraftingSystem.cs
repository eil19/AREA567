using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CraftingSystem : MonoBehaviour
{
    public const int GRID_SIZE = 9;

    [Header("References")]
    [SerializeField] private Inventory inventory;

    [Header("Recipes")]
    [SerializeField] private List<CraftingRecipe> craftingRecipesList = new List<CraftingRecipe>();

    [Header("Events")]
    public UnityEvent OnGridChanged;
    public UnityEvent OnCraftingSucceeded;

    private CraftingGridSlot[] craftingGrid = new CraftingGridSlot[GRID_SIZE];
    private CraftingRecipe currentRecipe;
    public CraftingGridSlot[] CraftingGrid => craftingGrid;
    public CraftingRecipe CurrentRecipe => currentRecipe;

    private void Awake()
    {
        InitialiseGrid();
    }

    private void Start()
    {
        if (inventory == null)
        {
            GameObject inventoryObject = GameObject.Find("Inventory");
            if (inventoryObject != null)
            {
                inventory = inventoryObject.GetComponent<Inventory>();
            }
        }
        CheckCraftingOutput();
    }

    private void InitialiseGrid()
    {
        for (int i = 0; i < craftingGrid.Length; i++)
        {
            craftingGrid[i] = new CraftingGridSlot();
        }
    }

    public CraftingGridSlot GetGridSlot(int index)
    {
        if (index < 0 || index >= craftingGrid.Length) return null;
        return craftingGrid[index];
    }

    public bool PlaceItem(int gridIndex, ItemData itemData, int quantity)
    {
        if (gridIndex < 0 || gridIndex >= craftingGrid.Length) return false;
        if (itemData == null || quantity <= 0) return false;

        CraftingGridSlot slot = craftingGrid[gridIndex];
        if (!slot.IsEmpty && slot.ItemData != itemData) return false;

        if (slot.IsEmpty)
        {
            slot.SetItem(itemData, quantity);
        }
        else
        {
            slot.quantity += quantity;
        }

        GridChanged();
        return true;
    }

    public void RemoveFromGrid(int gridIndex, int quantity)
    {
        if (gridIndex < 0 || gridIndex >= craftingGrid.Length) return;

        CraftingGridSlot slot = craftingGrid[gridIndex];
        if (slot.IsEmpty) return;

        slot.quantity -= quantity;
        if (slot.quantity <= 0)
        {
            slot.Clear();
        }

        GridChanged();
    }

    public void ClearGridSlot(int gridIndex)
    {
        if (gridIndex < 0 || gridIndex >= craftingGrid.Length) return;
        craftingGrid[gridIndex].Clear();
        GridChanged();
    }

    private void GridChanged()
    {
        CheckCraftingOutput();
        OnGridChanged?.Invoke();
    }

    // check crafting output
    public void CheckCraftingOutput()
    {
        currentRecipe = null;
        foreach (CraftingRecipe recipe in craftingRecipesList)
        {
            if (RecipeMatches(recipe))
            {
                currentRecipe = recipe;
                return;
            }
        }
    }

    private bool RecipeMatches(CraftingRecipe recipe)
    {
        if (recipe == null || recipe.recipeGrid == null ||
            recipe.recipeGrid.Length != GRID_SIZE) return false;
        
        for (int i = 0; i < GRID_SIZE; i++)
        {
            CraftingIngredient required = recipe.recipeGrid[i];
            CraftingGridSlot actual = craftingGrid[i];

            bool requiresNothing =
                required == null || required.item == null || required.quantity <= 0;

            if (requiresNothing)
            {
                if (!actual.IsEmpty) return false;
                continue;
            }
            if (actual.IsEmpty) return false;
            if (actual.ItemData != required.item) return false;
            if (actual.quantity < required.quantity) return false;
        }
        return true;
    }

    public bool CraftCurrentRecipe()
    {
        if (currentRecipe == null || inventory == null) return false;
        CraftingRecipe recipe = currentRecipe;

        // consume required ingredients
        for (int i = 0; i < GRID_SIZE; i++)
        {
            CraftingIngredient required = recipe.recipeGrid[i];
            if (required == null || required.item == null || required.quantity <= 0) continue;
            craftingGrid[i].quantity -= required.quantity;
            if (craftingGrid[i].quantity <= 0)
            {
                craftingGrid[i].Clear();
            }
        }

        // add crafted item to inventory
        ItemInstance craftedItem = new ItemInstance(recipe.outputItem, null, recipe.outputQuantity);
        bool added = inventory.AddItem(craftedItem);
        if (!added)
        {
            Debug.Log("Crafted item could not be added as inventory is full");
            return false;
        }

        Debug.Log("Crafted: " + recipe.outputItem.itemName);
        OnCraftingSucceeded?.Invoke();
        GridChanged();
        return true;
    }
}
