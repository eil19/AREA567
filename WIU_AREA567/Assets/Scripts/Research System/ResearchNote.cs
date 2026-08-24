using UnityEngine;

// Mirrors Item.cs's IPickupable pattern - a physical Research Note object
// in the world. On pickup, adds to the persistent ResearchLog instead of
// Sze Yee's Inventory - Research Notes don't take up one of the 5 physical
// item slots, they're knowledge, not a resource.
public class ResearchNote : MonoBehaviour, IPickupable
{
    [SerializeField] private ResearchData researchData;

    private void Start()
    {
        if (researchData != null && researchData.icon != null)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = researchData.icon;
            }
        }
    }

    public void Pickup(GameObject picker)
    {
        if (researchData == null) return;

        bool added = ResearchLog.Instance.AddResearch(researchData);
        if (added)
        {
            Debug.Log($"{picker.name} discovered research: {researchData.researchName}");
            NotificationPopupUI.Instance?.Show("New note unlocked!");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"Already discovered: {researchData.researchName}");
        }
    }
}