using UnityEngine;

// Simple test object to verify PlayerInteractor's Pickup detection works.
// Swap for real pickups (ResearchNote, Scrap, Chemical) once those exist.
public class TestPickupable : MonoBehaviour, IPickupable
{
    [SerializeField] private string itemName = "Test Item";

    public void Pickup(GameObject picker)
    {
        Debug.Log($"{picker.name} picked up {itemName}");
        Destroy(gameObject);
    }
}