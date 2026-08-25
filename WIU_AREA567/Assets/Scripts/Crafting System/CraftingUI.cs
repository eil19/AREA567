using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CraftingSystem craftingSystem;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Image dragIcon;

    [Header("CraftingSlots")]
    [SerializeField] private CraftingSlotUI[] craftingSlots;

    [Header("Output")]
    [SerializeField] private Image outputImage;
    [SerializeField] private TMP_Text outputQuantity;

    private void Start()
    {
        if (craftingSystem == null)
        {
            craftingSystem = FindFirstObjectByType<CraftingSystem>();
        }
        if (inventory == null)
        {
            inventory = FindFirstObjectByType<Inventory>();
        }
        if (craftingSystem == null)
        {
            Debug.Log("Crafting system not found");
            return;
        }

        InitialiseSlots();

        if (craftingSystem != null)
        {
            craftingSystem.OnGridChanged.AddListener(RefreshCraftingUI);
        }

        RefreshCraftingUI();
    }

    private void OnDestroy()
    {
        if (craftingSystem != null)
        {
            craftingSystem.OnGridChanged.RemoveListener(RefreshCraftingUI);
        }
    }

    private void InitialiseSlots()
    {
        for (int i = 0; i < craftingSlots.Length; i++)
        {
            craftingSlots[i].Initialise(
                i, craftingSystem, inventory, dragIcon);
        }
    }

    public void RefreshCraftingUI()
    {
        for (int i = 0; i < craftingSlots.Length; i++)
        {
            CraftingGridSlot slot = craftingSystem.GetGridSlot(i);
            craftingSlots[i].UpdateSlot(slot);
        }

        RefreshOutput();
    }

    private void RefreshOutput()
    {
        CraftingRecipe recipe = craftingSystem.CurrentRecipe;

        if (recipe == null)
        {
            outputImage.enabled = false;
            outputImage.sprite = null;
            outputQuantity.text = "";
            return;
        }

        outputImage.enabled = true;
        outputImage.sprite = recipe.outputItem.itemImage;
        outputQuantity.text =
            recipe.outputQuantity > 1 ?
            "x" + recipe.outputQuantity : "";
    }

    public void CraftOutput()
    {
        craftingSystem.CraftCurrentRecipe();
    }
}
