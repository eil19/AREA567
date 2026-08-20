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

    private Vector2 moveInput;

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
        moveInput = InputSystem.actions["Move"].ReadValue<Vector2>();

        bool isMoving = moveInput.sqrMagnitude > 0.01f;
        animator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            FacingDirection = moveInput.normalized;

            // Feed these to a Blend Tree for 8-directional animation,
            // or round to nearest cardinal direction in the Animator
            // if using a simpler 4-direction sprite set.
            animator.SetFloat("MoveX", FacingDirection.x);
            animator.SetFloat("MoveY", FacingDirection.y);
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

        body.linearVelocity = moveInput.normalized * speed;
    }
}