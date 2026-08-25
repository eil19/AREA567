using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerOrganisation : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    [Header("Carrying")]
    [SerializeField] private Transform holdPoint;

    [Header("Detection (mirrors PlayerInteractor's range/radius pattern)")]
    [SerializeField] private float actionRange = 1f;
    [SerializeField] private float actionRadius = 0.5f;
    [SerializeField] private LayerMask organisableLayer;
    [SerializeField] private LayerMask placementZoneLayer;

    private OrganisableItem carriedItem;
    private Rigidbody2D carriedBody;
    private Collider2D carriedCollider;

    public bool IsCarrying => carriedItem != null;

    void Awake()
    {
        if (playerController == null) playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (InputSystem.actions["Pickup"].WasPressedThisFrame())
        {
            if (carriedItem == null)
            {
                TryPickUp();
            }
            else
            {
                TryPlaceDown();
            }
        }

        if (carriedItem != null)
        {
            carriedItem.transform.position = holdPoint != null ? holdPoint.position : GetCheckPoint();
        }
    }

    private Vector2 GetCheckPoint()
    {
        return (Vector2)transform.position + playerController.FacingDirection * actionRange;
    }

    private void TryPickUp()
    {
        Collider2D hit = Physics2D.OverlapCircle(GetCheckPoint(), actionRadius, organisableLayer);
        if (hit == null) return;
        if (!hit.TryGetComponent(out OrganisableItem item)) return;

        carriedItem = item;
        carriedCollider = hit;
        carriedBody = hit.GetComponent<Rigidbody2D>();

        // Disable physics/collision while carried so it doesn't interfere with player/pick up twicw
        if (carriedCollider != null) carriedCollider.enabled = false;
        if (carriedBody != null) carriedBody.simulated = false;
    }

    private void TryPlaceDown()
    {
        Vector2 dropPoint = GetCheckPoint();
        Collider2D zoneHit = Physics2D.OverlapCircle(dropPoint, actionRadius, placementZoneLayer);

        carriedItem.transform.position = dropPoint;

        if (carriedCollider != null) carriedCollider.enabled = true;
        if (carriedBody != null) carriedBody.simulated = true;

        bool correctZone = zoneHit != null
            && zoneHit.TryGetComponent(out PlacementZone zone)
            && zone.ZoneId == carriedItem.ZoneId;

        carriedItem.SetOrganised(correctZone);

        carriedItem = null;
        carriedCollider = null;
        carriedBody = null;
    }

    void OnDrawGizmosSelected()
    {
        if (playerController == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(GetCheckPoint(), actionRadius);
    }
}
