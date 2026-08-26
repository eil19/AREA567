using UnityEngine;
using UnityEngine.Events;

public class CraftingBench : MonoBehaviour,
    IInteractable
{
    [SerializeField] private GameObject craftingPanel;
    [SerializeField] private GameObject recipePanel;
    [SerializeField] private CraftingSystem craftingSystem;

    [Header("Events")]
    public UnityEvent OnCraftingOpened;
    public UnityEvent OnCraftingClosed;

    private void Start()
    {
        if (craftingSystem == null)
        {
            craftingSystem = FindFirstObjectByType<CraftingSystem>();
        }
    }

    public void Interact(GameObject interactor)
    {
        OpenCrafting();
    }

    public void OpenCrafting()
    {
        if (craftingPanel != null)
        {
            craftingPanel.SetActive(true);
        }

        if (recipePanel != null)
        {
            recipePanel.SetActive(true);
        }
        OnCraftingOpened?.Invoke();
    }

    public void CloseCrafting()
    {
        if (craftingSystem != null)
        {
            bool returnedAll = craftingSystem.ReturnAllItemsToInventory();
            if (!returnedAll)
            {
                Debug.Log("Cannot close crafting until all items return to inventory");
                return;
            }
        }
        if (craftingPanel != null)
        {
            craftingPanel.SetActive(false);
        }
        if (recipePanel != null) 
        { 
            recipePanel.SetActive(false); 
        }
        OnCraftingClosed?.Invoke();
    }
}