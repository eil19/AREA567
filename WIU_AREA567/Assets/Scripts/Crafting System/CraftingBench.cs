using UnityEngine;

public class CraftingBench : MonoBehaviour,
    IInteractable
{
    [SerializeField] private GameObject craftingPanel;
    [SerializeField] private GameObject recipePanel;

    public void Interact(GameObject interactor)
    {
        if (craftingPanel != null)
        {
            craftingPanel.SetActive(true);
        }
        if (recipePanel != null)
        {
            recipePanel.SetActive(true);
        }
    }

    public void CloseCrafting()
    {
        craftingPanel.SetActive(false);
        recipePanel.SetActive(false);
    }
}