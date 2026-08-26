using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

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
    public UnityEvent<GameObject> OnInteractableFocused;
    public UnityEvent OnInteractableLostFocus;

    [Header("Pickup Focus Events")]
    public UnityEvent<GameObject> OnPickupFocused;
    public UnityEvent OnPickupLostFocus;

    private GameObject currentFocusedInteractable;
    private GameObject currentFocusedPickup;

    private bool interactionLocked;
    public bool IsInteractionLocked => interactionLocked;

    public void SetInteractionLocked(bool locked)
    {
        interactionLocked = locked;
        if (locked)
        {
            if (currentFocusedInteractable != null)
            {
                OnInteractableLostFocus?.Invoke();
                currentFocusedInteractable = null;
            }
            if (currentFocusedPickup != null)
            {
                OnPickupLostFocus?.Invoke();
                currentFocusedPickup = null;
            }
        }
    }

    void Awake()
    {
        if (playerController == null) playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (interactionLocked) return;

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
            if (interactHit != null && interactHit.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact(gameObject);
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

                // The picked-up object may destroy itself this same frame
                // (ResearchNote does). Don't rely on next frame's overlap
                // check to notice - a reference to a just-destroyed
                // GameObject compares equal to null via Unity's overloaded
                // operators, which silently breaks the change-detection
                // above. Force the lost-focus event now instead.
                if (currentFocusedPickup != null)
                {
                    OnPickupLostFocus?.Invoke();
                    currentFocusedPickup = null;
                }
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