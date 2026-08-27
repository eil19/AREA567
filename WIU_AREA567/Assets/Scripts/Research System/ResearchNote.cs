using UnityEngine;

// Mirrors Item.cs's IPickupable pattern (a physical Research Note object tat doesnt take up space in inventory)

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
        if (researchData == null)
            return;

        bool added =
            ResearchLog.Instance.AddResearch(
                researchData
            );

        if (added)
        {
            Debug.Log(
                picker.name +
                " discovered research: " +
                researchData.researchName
            );

            if (researchData.category ==
                    ResearchCategory.Recipe &&
                researchData.unlockedRecipe != null)
            {
                NotificationPopupUI.Instance?.Show(
                    "Recipe Unlocked: " +
                    researchData.unlockedRecipe
                        .recipeName
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
        else
        {
            Debug.Log(
                "Already discovered: " +
                researchData.researchName
            );
        }
    }
}