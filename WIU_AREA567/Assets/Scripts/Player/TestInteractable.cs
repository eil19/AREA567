using UnityEngine;

// Simple test object to verify PlayerInteractor's detection + polymorphic
// Interact() call work correctly. Swap for real interactables (crafting
// bench, door, NPC) once those systems are built by the team.
public class TestInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string message = "Interacted!";
    [SerializeField] private bool destroyOnInteract = false;

    public void Interact(GameObject interactor)
    {
        Debug.Log($"{interactor.name} interacted with {gameObject.name}: {message}");

        if (destroyOnInteract)
        {
            Destroy(gameObject);
        }
    }
}