using UnityEngine;

public class OrganisableItem : MonoBehaviour
{
    [SerializeField] private string zoneId;
    public string ZoneId => zoneId;

    [Header("Visual Feedback")]
    [SerializeField] private Color correctColor = Color.white;
    [SerializeField] private Color incorrectColor = Color.red;

    private SpriteRenderer spriteRenderer;

    public bool IsOrganised { get; private set; }

    // False until the player has placed it at least once - keeps items
    // that haven't been touched yet from counting as "wrong" / glowing red.
    public bool HasBeenPlaced { get; private set; }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        OrganisationManager.Instance?.Register(this);
    }

    void OnDestroy()
    {
        OrganisationManager.Instance?.Unregister(this);
    }

    public void SetOrganised(bool organised)
    {
        HasBeenPlaced = true;
        IsOrganised = organised;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = organised ? correctColor : incorrectColor;
        }

        OrganisationManager.Instance?.NotifyChanged();
    }
}
