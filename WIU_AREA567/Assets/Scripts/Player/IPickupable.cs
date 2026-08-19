using UnityEngine;

// Implement this on anything the player can pick up directly into their
// inventory - Research Notes, Scrap, Chemical, etc. Kept separate from
// IInteractable since picking something up is functionally different from
// interacting with it in place (crafting bench, cryo tube panel, NPCs).
public interface IPickupable
{
    void Pickup(GameObject picker);
}