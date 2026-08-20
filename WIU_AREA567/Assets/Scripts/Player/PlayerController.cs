using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4f;

    [Header("Stealth")]
    public float stealthSpeedMultiplier = 0.5f;
    private bool isStealthed;
    public bool IsStealthed => isStealthed;

    [Header("Attack Point (for AttackEventHandler)")]
    [Tooltip("Child transform used by AttackEventHandler - this script repositions it to face FacingDirection every frame. AttackEventHandler reads this via the AttackPoint property, so it only needs to be assigned here, not duplicated.")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackPointDistance = 0.6f;
    public Transform AttackPoint => attackPoint;

    private Animator animator;
    private Rigidbody2D body;

    private Vector2 moveInput; // now always a cardinal unit vector or zero - see Update()
    private int directionIndex = 0; // 0 = Down, 1 = Up, 2 = Side - matches Animator's "Direction" Blend Tree

    // Defaults facing down - typical top-down convention (character faces camera at rest).
    // IMPORTANT: only updates while actively moving (see Update() below) - standing still
    // keeps whatever direction was last faced. Anything reading this (AttackEventHandler,
    // PlayerInteractor) inherits that behavior.
    public Vector2 FacingDirection { get; private set; } = Vector2.down;

    void Awake()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();

        // Top-down: no gravity should affect the player
        body.gravityScale = 0f;
    }

    void Update()
    {
        Vector2 rawInput = InputSystem.actions["Move"].ReadValue<Vector2>();

        // Snap to 4 cardinal directions - no diagonal movement allowed.
        // Whichever axis has the larger magnitude wins; the other is zeroed
        // out entirely, so moveInput is always exactly (±1,0), (0,±1), or (0,0).
        if (rawInput.sqrMagnitude < 0.01f)
        {
            moveInput = Vector2.zero;
        }
        else if (Mathf.Abs(rawInput.x) > Mathf.Abs(rawInput.y))
        {
            moveInput = new Vector2(Mathf.Sign(rawInput.x), 0f);
        }
        else
        {
            moveInput = new Vector2(0f, Mathf.Sign(rawInput.y));
        }

        bool isMoving = moveInput.sqrMagnitude > 0.01f;
        animator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            FacingDirection = moveInput; // already a cardinal unit vector, no normalization needed

            animator.SetFloat("MoveX", FacingDirection.x);
            animator.SetFloat("MoveY", FacingDirection.y);

            // Direction is now always purely vertical or purely horizontal
            // (movement itself is restricted to 4 directions above), so this
            // maps straight onto the 3 Blend Tree poses - Side still covers
            // both Left and Right via the localScale flip.
            if (FacingDirection.y != 0)
            {
                directionIndex = FacingDirection.y > 0 ? 1 : 0; // Up : Down
            }
            else
            {
                directionIndex = 2; // Side
                transform.localScale = new Vector3(FacingDirection.x < 0 ? -1f : 1f, 1f, 1f);
            }

            animator.SetFloat("Direction", directionIndex);
        }

        if (attackPoint != null)
        {
            attackPoint.localPosition = FacingDirection * attackPointDistance;
        }

        // Stealth - toggle on press
        if (InputSystem.actions["Stealth"].WasPressedThisFrame())
        {
            isStealthed = !isStealthed;
            animator.SetBool("IsStealthed", isStealthed);
        }
    }

    void FixedUpdate()
    {
        float speed = moveSpeed;
        if (isStealthed) speed *= stealthSpeedMultiplier;

        body.linearVelocity = moveInput * speed; // moveInput is already a unit vector (or zero)
    }
}