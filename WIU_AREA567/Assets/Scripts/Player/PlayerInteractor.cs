using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

// Handles two separate player-facing systems, both using the same
// facing-direction + range/radius detection:
//   - Interact (E)         -> IInteractable.Interact()  - crafting bench, cryo tube panel, NPCs
//   - Pickup (Right-Click) -> IPickupable.Pickup()      - Research Notes, Scrap, Chemical
// Each has its own Layer Mask and focus events (fired when something enters/
// leaves range so UI can show "Press E" / "Right-click to pick up" prompts
// without polling this script - see FloatingPromptUI).
[RequireComponent(typeof(PlayerController))]
public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    [Header("Shared detection range (used by BOTH Interact and Pickup checks below)")]
    [SerializeField] private float interactRange = 1f;
    [SerializeField] private float interactRadius = 0.5f;

    [Header("Interact (E) - crafting bench, cryo tube panel, NPCs, etc.")]
    [SerializeField] private LayerMask interactableLayer;

    [Header("Pickup (Right Click) - Research Notes, Scrap, Chemical, etc.")]
    [SerializeField] private LayerMask pickupLayer;

    [Header("Interact Focus Events")]
    [Tooltip("Fires when an interactable enters range/facing. Passes the interactable's GameObject - use to show a 'Press E' prompt.")]
    public UnityEvent<GameObject> OnInteractableFocused;
    [Tooltip("Fires when the previously focused interactable leaves range/facing. Use to hide the prompt.")]
    public UnityEvent OnInteractableLostFocus;

    [Header("Pickup Focus Events")]
    [Tooltip("Fires when a pickup enters range/facing. Passes the pickup's GameObject - use to show a 'Right-click to pick up' prompt.")]
    public UnityEvent<GameObject> OnPickupFocused;
    [Tooltip("Fires when the previously focused pickup leaves range/facing. Use to hide the prompt.")]
    public UnityEvent OnPickupLostFocus;

    private GameObject currentFocusedInteractable;
    private GameObject currentFocusedPickup;

    void Awake()
    {
        if (playerController == null) playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        // --- Interact (E) ---
        Collider2D interactHit = CheckInRange(interactableLayer);
        GameObject interactObject = interactHit != null ? interactHit.gameObject : null;

        if (interactObject != currentFocusedInteractable)
        {
            if (currentFocusedInteractable != null) OnInteractableLostFocus?.Invoke();
            if (interactObject != null) OnInteractableFocused?.Invoke(interactObject);
            currentFocusedInteractable = interactObject;
        }

        if (InputSystem.actions["Interact"].WasPressedThisFrame())
        {
            Debug.Log($"[PlayerInteractor] Interact pressed. interactHit = {interactHit}");

            if (interactHit != null && interactHit.TryGetComponent(out IInteractable interactable))
            {
                if (interactable.CanInteract())
                {
                    interactable.Interact(gameObject);
                }
                else
                {
                    Debug.Log("[PlayerInteractor] Object found, but CanInteract() returned false.");
                }
            }
        }

        // --- Pickup (Right Click) ---
        Collider2D pickupHit = CheckInRange(pickupLayer);
        GameObject pickupObject = pickupHit != null ? pickupHit.gameObject : null;

        if (pickupObject != currentFocusedPickup)
        {
            if (currentFocusedPickup != null) OnPickupLostFocus?.Invoke();
            if (pickupObject != null) OnPickupFocused?.Invoke(pickupObject);
            currentFocusedPickup = pickupObject;
        }

        if (InputSystem.actions["Pickup"].WasPressedThisFrame())
        {
            if (pickupHit != null && pickupHit.TryGetComponent(out IPickupable pickupable))
            {
                pickupable.Pickup(gameObject);
            }
        }
    }

    private Collider2D CheckInRange(LayerMask layer)
    {
        Vector2 checkPoint = (Vector2)transform.position + playerController.FacingDirection * interactRange;
        return Physics2D.OverlapCircle(checkPoint, interactRadius, layer);
    }

    void OnDrawGizmosSelected()
    {
        if (playerController == null) return;
        Gizmos.color = Color.cyan;
        Vector2 checkPoint = (Vector2)transform.position + playerController.FacingDirection * interactRange;
        Gizmos.DrawWireSphere(checkPoint, interactRadius);
    }
}