using UnityEngine;

// Implement this on any object the player can interact with in place -
// crafting bench, control panel, door, NPC, etc. For items that get picked
// up directly into the inventory (Research Notes, Scrap, Chemical), use
// IPickupable instead.
public interface IInteractable
{
    void Interact(GameObject interactor);
    bool CanInteract();
}