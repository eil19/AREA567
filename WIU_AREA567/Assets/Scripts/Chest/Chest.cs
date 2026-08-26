using UnityEngine;
using UnityEngine.Events;

public class Chest : MonoBehaviour,
    IInteractable
{
    [SerializeField] private GameObject chestPanel;
    public UnityEvent OnChestOpened;

    public void Interact(GameObject interactor)
    {
        chestPanel.SetActive(true);
        OnChestOpened?.Invoke();
    }
}
