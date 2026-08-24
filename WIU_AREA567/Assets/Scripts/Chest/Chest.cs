using UnityEngine;

public class Chest : MonoBehaviour,
    IInteractable
{
    [SerializeField]
    private GameObject chestPanel;

    public void Interact(GameObject interactor)
    {
        chestPanel.SetActive(true);
    }
}
