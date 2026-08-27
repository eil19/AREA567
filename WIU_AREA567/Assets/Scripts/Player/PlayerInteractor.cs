using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerController))]
public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    [Header("Shared Detection Range")]
    [SerializeField] private float interactRange = 1f;
    [SerializeField] private float interactRadius = 0.5f;

    [Header("Interact (E)")]
    [SerializeField] private LayerMask interactableLayer;

    [Header("Pickup (R)")]
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

    private void Awake()
    {
        if (playerController == null)
        {
            playerController =
                GetComponent<PlayerController>();
        }
    }

    private void Update()
    {
        if (interactionLocked)
            return;

        UpdateInteractable();
        UpdatePickup();
    }

    public void SetInteractionLocked(bool locked)
    {
        interactionLocked = locked;

        if (!locked)
            return;

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

    // =========================
    // INTERACT
    // =========================

    private void UpdateInteractable()
    {
        Collider2D interactHit =
            CheckInRange(interactableLayer);

        GameObject interactObject =
            GetInteractableObject(interactHit);

        // Focus changed.
        if (interactObject !=
            currentFocusedInteractable)
        {
            if (currentFocusedInteractable != null)
            {
                OnInteractableLostFocus?.Invoke();
            }

            currentFocusedInteractable =
                interactObject;

            if (currentFocusedInteractable != null)
            {
                OnInteractableFocused?.Invoke(
                    currentFocusedInteractable
                );
            }
        }

        // Press E.
        if (!InputSystem.actions["Interact"]
            .WasPressedThisFrame())
        {
            return;
        }

        if (interactHit == null)
            return;

        IInteractable interactable =
            GetInteractable(interactHit);

        if (interactable != null)
        {
            interactable.Interact(gameObject);
        }
    }

    private IInteractable GetInteractable(
        Collider2D hit)
    {
        if (hit == null)
            return null;

        IInteractable interactable =
            hit.GetComponent<IInteractable>();

        if (interactable == null)
        {
            interactable =
                hit.GetComponentInParent<
                    IInteractable>();
        }

        return interactable;
    }

    private GameObject GetInteractableObject(
        Collider2D hit)
    {
        if (hit == null)
            return null;

        IInteractable interactable =
            GetInteractable(hit);

        MonoBehaviour behaviour =
            interactable as MonoBehaviour;

        // Follow the object containing the
        // IInteractable script instead of
        // a random child collider.
        if (behaviour != null)
        {
            return behaviour.gameObject;
        }

        return hit.gameObject;
    }

    // =========================
    // PICKUP
    // =========================

    private void UpdatePickup()
    {
        Collider2D pickupHit =
            CheckInRange(pickupLayer);

        GameObject pickupObject =
            GetPickupObject(pickupHit);

        if (pickupObject !=
            currentFocusedPickup)
        {
            if (currentFocusedPickup != null)
            {
                OnPickupLostFocus?.Invoke();
            }

            currentFocusedPickup =
                pickupObject;

            if (currentFocusedPickup != null)
            {
                OnPickupFocused?.Invoke(
                    currentFocusedPickup
                );
            }
        }

        if (!InputSystem.actions["Pickup"]
            .WasPressedThisFrame())
        {
            return;
        }

        if (pickupHit == null)
            return;

        IPickupable pickupable =
            GetPickupable(pickupHit);

        if (pickupable == null)
            return;

        pickupable.Pickup(gameObject);

        if (currentFocusedPickup != null)
        {
            OnPickupLostFocus?.Invoke();
            currentFocusedPickup = null;
        }
    }

    private IPickupable GetPickupable(
        Collider2D hit)
    {
        if (hit == null)
            return null;

        IPickupable pickupable =
            hit.GetComponent<IPickupable>();

        if (pickupable == null)
        {
            pickupable =
                hit.GetComponentInParent<
                    IPickupable>();
        }

        return pickupable;
    }

    private GameObject GetPickupObject(
        Collider2D hit)
    {
        if (hit == null)
            return null;

        IPickupable pickupable =
            GetPickupable(hit);

        MonoBehaviour behaviour =
            pickupable as MonoBehaviour;

        if (behaviour != null)
        {
            return behaviour.gameObject;
        }

        return hit.gameObject;
    }

    // =========================
    // DETECTION
    // =========================

    private Collider2D CheckInRange(
        LayerMask layer)
    {
        Vector2 checkPoint =
            (Vector2)transform.position +
            playerController.FacingDirection *
            interactRange;

        return Physics2D.OverlapCircle(
            checkPoint,
            interactRadius,
            layer
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (playerController == null)
            return;

        Gizmos.color = Color.cyan;

        Vector2 checkPoint =
            (Vector2)transform.position +
            playerController.FacingDirection *
            interactRange;

        Gizmos.DrawWireSphere(
            checkPoint,
            interactRadius
        );
    }
}