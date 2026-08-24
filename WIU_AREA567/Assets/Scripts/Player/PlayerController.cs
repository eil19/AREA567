using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
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
    private SpriteRenderer spriteRenderer;

    private Vector2 moveInput;
    private Vector2 previousRawInput = Vector2.zero;
    private bool horizontalWasLastPressed = false;
    private int directionIndex = 0; // 0 = Down, 1 = Up, 2 = Side - matches Animator's "Direction" parameter

    // Defaults facing down - typical top-down convention (character faces camera at rest).
    // IMPORTANT: only updates while actively moving (see Update() below) - standing still
    // keeps whatever direction was last faced. Anything reading this (AttackEventHandler,
    // PlayerInteractor) inherits that behavior.
    public Vector2 FacingDirection { get; private set; } = Vector2.down;

    void Awake()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Top-down: no gravity should affect the player
        body.gravityScale = 0f;
    }

    void Update()
    {
        Vector2 rawInput = InputSystem.actions["Move"].ReadValue<Vector2>();

        bool xPressedThisFrame = Mathf.Abs(rawInput.x) > 0.01f && Mathf.Abs(previousRawInput.x) <= 0.01f;
        bool yPressedThisFrame = Mathf.Abs(rawInput.y) > 0.01f && Mathf.Abs(previousRawInput.y) <= 0.01f;

        if (xPressedThisFrame) horizontalWasLastPressed = true;
        if (yPressedThisFrame) horizontalWasLastPressed = false;

        if (rawInput.sqrMagnitude < 0.01f)
        {
            moveInput = Vector2.zero;
        }
        else if (Mathf.Abs(rawInput.x) <= 0.01f)
        {
            moveInput = new Vector2(0f, Mathf.Sign(rawInput.y));
            horizontalWasLastPressed = false;
        }
        else if (Mathf.Abs(rawInput.y) <= 0.01f)
        {
            moveInput = new Vector2(Mathf.Sign(rawInput.x), 0f);
            horizontalWasLastPressed = true;
        }
        else
        {
            moveInput = horizontalWasLastPressed
                ? new Vector2(Mathf.Sign(rawInput.x), 0f)
                : new Vector2(0f, Mathf.Sign(rawInput.y));
        }

        previousRawInput = rawInput;

        bool isMoving = moveInput.sqrMagnitude > 0.01f;
        animator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            FacingDirection = moveInput;

            animator.SetFloat("MoveX", FacingDirection.x);
            animator.SetFloat("MoveY", FacingDirection.y);

            if (FacingDirection.y != 0)
            {
                directionIndex = FacingDirection.y > 0 ? 1 : 0; // Up : Down
            }
            else
            {
                directionIndex = 2; // Side
                // Flip the sprite visually only - Transform itself never flips,
                // so AttackPoint's child-local-position math stays correct in
                // every direction (this used to flip transform.localScale.x,
                // which broke AttackPoint's world position when facing left).
                spriteRenderer.flipX = FacingDirection.x < 0;
            }

            animator.SetInteger("Direction", directionIndex);
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

        // Attack / Taser
        if (InputSystem.actions["Attack"].WasPressedThisFrame())
        {
            animator.SetBool("IsBusy", true);
            animator.SetTrigger("Attack");
        }

        if (InputSystem.actions["Taser"].WasPressedThisFrame())
        {
            animator.SetBool("IsBusy", true);
            animator.SetTrigger("Taser");
        }
    }

    void FixedUpdate()
    {
        float speed = moveSpeed;
        if (isStealthed) speed *= stealthSpeedMultiplier;

        body.linearVelocity = moveInput * speed;
    }
}