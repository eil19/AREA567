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
            if (interactHit != null)
            {
                IInteractable interactable =
                    interactHit
                        .GetComponent<IInteractable>();

                if (interactable == null)
                {
                    interactable =
                        interactHit
                            .GetComponentInParent<
                                IInteractable>();
                }

                interactable?.Interact(
                    gameObject
                );
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
            if (pickupHit != null)
            {
                IPickupable pickupable =
                    pickupHit
                        .GetComponent<IPickupable>();

                if (pickupable == null)
                {
                    pickupable =
                        pickupHit
                            .GetComponentInParent<
                                IPickupable>();
                }

                if (pickupable != null)
                {
                    pickupable.Pickup(gameObject);

                    if (currentFocusedPickup != null)
                    {
                        OnPickupLostFocus?.Invoke();
                        currentFocusedPickup = null;
                    }
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