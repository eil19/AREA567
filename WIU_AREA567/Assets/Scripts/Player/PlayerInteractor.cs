using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private float interactRange = 1f;
    [SerializeField] private float interactRadius = 0.5f;
    [SerializeField] private LayerMask interactableLayer;

    void Awake()
    {
        if (playerController == null) playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (InputSystem.actions["Interact"].WasPressedThisFrame())
        {
            Vector2 checkPoint = (Vector2)transform.position + playerController.FacingDirection * interactRange;
            Collider2D hit = Physics2D.OverlapCircle(checkPoint, interactRadius, interactableLayer);

            if (hit != null && hit.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact(gameObject);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (playerController == null) return;
        Gizmos.color = Color.cyan;
        Vector2 checkPoint = (Vector2)transform.position + playerController.FacingDirection * interactRange;
        Gizmos.DrawWireSphere(checkPoint, interactRadius);
    }
}
