using UnityEngine;

// Mirrors Item.cs's IPickupable pattern (a physical Research Note object tat doesnt take up space in inventory)

public class ResearchNote : MonoBehaviour, IPickupable
{
    [SerializeField] private ResearchData researchData;

    private void Start()
    {
        if (researchData == null)
        {
            return;
        }

        // If this note was already collected
        // during the current run, remove the
        // newly-loaded world copy immediately.
        if (ResearchLog.Instance != null &&
            ResearchLog.Instance.HasDiscovered(
                researchData))
        {
            Destroy(gameObject);
            return;
        }

        if (researchData.icon != null)
        {
            SpriteRenderer spriteRenderer =
                GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite =
                    researchData.icon;
            }
        }
    }

    public void Pickup(GameObject picker)
    {
        if (researchData == null)
        {
            return;
        }

        ResearchLog researchLog =
            ResearchLog.Instance;

        if (researchLog == null)
        {
            Debug.LogError(
                "ResearchNote: ResearchLog not found."
            );

            return;
        }

        // Already discovered?
        if (researchLog.HasDiscovered(
            researchData))
        {
            Destroy(gameObject);
            return;
        }

        bool added =
            researchLog.AddResearch(
                researchData
            );

        if (!added)
        {
            return;
        }

        Debug.Log(
            "Discovered research: " +
            researchData.researchName
        );

        if (researchData.category ==
                ResearchCategory.Recipe &&
            researchData.unlockedRecipe != null)
        {
            NotificationPopupUI.Instance?.Show(
                "Recipe Unlocked: " +
                researchData.unlockedRecipe.recipeName
            );
        }
        else
        {
            NotificationPopupUI.Instance?.Show(
                "Research Discovered: " +
                researchData.researchName
            );
        }

        Destroy(gameObject);
    }
}