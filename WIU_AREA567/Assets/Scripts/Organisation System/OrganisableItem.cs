using UnityEngine;

public class OrganisableItem : MonoBehaviour
{
    [Header("Organisation")]
    [SerializeField] private string zoneId;
    public string ZoneId => zoneId;

    [Header("Persistence")]
    [SerializeField] private string persistentID;

    [Header("Visual Feedback")]
    [SerializeField] private Color correctColor = Color.white;
    [SerializeField] private Color incorrectColor = Color.red;

    private SpriteRenderer spriteRenderer;

    //public string ZoneId => zoneId;
    public bool IsOrganised { get; private set; }

    // False until the player has placed it at least once - keeps items
    // that haven't been touched yet from counting as "wrong" / glowing red.
    public bool HasBeenPlaced { get; private set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        RestoreState();

        OrganisationManager.Instance?.Register(this);
    }

    private void OnDestroy()
    {
        OrganisationManager.Instance?
            .Unregister(this);
    }

    public void SetOrganised(bool organised)
    {
        HasBeenPlaced = true;
        IsOrganised = organised;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = organised ? correctColor : incorrectColor;
        }

        if (organised)
        {
            OrganisationRunData.MarkOrganised(persistentID, transform.position);
        }
        else
        {
            OrganisationRunData.RemoveOrganised(persistentID);
        }

        OrganisationManager.Instance?.NotifyChanged();
    }

    private void RestoreState()
    {
        if (!OrganisationRunData.TryGetPosition(
            persistentID, out Vector3 savedPosition))
        {
            return;
        }

        transform.position = savedPosition;

        IsOrganised = true;
        HasBeenPlaced = true;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = correctColor;
        }
    }
}