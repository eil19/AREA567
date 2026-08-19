using UnityEngine;

// Implement this on any object the player can interact with -
// crafting bench, control panel, door, NPC, resource pickup, etc.
public interface IInteractable
{
    void Interact(GameObject interactor);
}
