using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    public enum WeaponType
    {
        Melee,
        Ranged,
        Taser
    }

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

    [Header("Ranged Spawn Point")]
    [Tooltip("Child transform used only for ranged projectiles. Its position is calculated separately from Attack Point.")]
    [SerializeField] private Transform rangedSpawnPoint;
    [SerializeField] private float rangedSpawnDistance = 0.6f;
    [Tooltip("Fine-tune the projectile muzzle position without affecting melee or taser range.")]
    [SerializeField] private Vector2 rangedSpawnOffset = new Vector2(0f, 0.15f);
    public Transform RangedSpawnPoint => rangedSpawnPoint;

    [Header("Weapons")]
    [Tooltip("KNOWN LIMITATION: currently a fixed mapping (1=Melee, 2=Ranged, 3=Taser). Once Sze Yee's 3-weapon-slot inventory allows reordering, this needs to read whichever weapon is actually in that slot instead of assuming a fixed type per number.")]
    [SerializeField] private WeaponType equippedWeapon = WeaponType.Melee;
    public WeaponType EquippedWeapon => equippedWeapon;

    private bool inputLocked;
    public bool IsInputLocked => inputLocked;

    public void SetInputLocked(bool locked)
    {
        inputLocked = locked;
        if (locked)
        {
            moveInput = Vector2.zero;
            previousRawInput = Vector2.zero;
            body.linearVelocity = Vector2.zero;
            animator.SetBool("IsMoving", false);
        }
    }

    public void SetSlowMultiplier(float multiplier)
    {
        externalSpeedMultiplier = multiplier;
    }

    public void ClearSlowMultiplier()
    {
        externalSpeedMultiplier = 1f;
    }

    private Animator animator;
    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private AttackEventHandler attackEventHandler;

    private Vector2 moveInput;
    private Vector2 previousRawInput = Vector2.zero;
    private bool horizontalWasLastPressed = false;
    private int directionIndex = 0; // 0 = Down, 1 = Up, 2 = Side - matches Animator's "Direction" parameter
    private float externalSpeedMultiplier = 1f;

    public Vector2 FacingDirection { get; private set; } = Vector2.down;

    void Awake()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        attackEventHandler = GetComponent<AttackEventHandler>();

        body.gravityScale = 0f;
    }

    void Update()
    {
        if (inputLocked) return;

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
                directionIndex = FacingDirection.y > 0 ? 1 : 0;
            }
            else
            {
                directionIndex = 2;
                spriteRenderer.flipX = FacingDirection.x < 0;
            }

            animator.SetInteger("Direction", directionIndex);
        }

        if (attackPoint != null)
        {
            attackPoint.localPosition = FacingDirection * attackPointDistance;
        }

        if (rangedSpawnPoint != null)
        {
            rangedSpawnPoint.localPosition = FacingDirection * rangedSpawnDistance + rangedSpawnOffset;
        }

        // Stealth - toggle on press
        if (InputSystem.actions["Stealth"].WasPressedThisFrame())
        {
            isStealthed = !isStealthed;
            animator.SetBool("IsStealthed", isStealthed);
        }

        HandleWeaponSelection();

        // Left mouse click (the existing Attack input action) uses the selected weapon.
        if (InputSystem.actions["Attack"].WasPressedThisFrame())
        {
            switch (equippedWeapon)
            {
                case WeaponType.Melee:
                    animator.SetBool("IsBusy", true);
                    animator.SetTrigger("Attack");
                    break;

                case WeaponType.Ranged:
                    attackEventHandler?.FireRangedAttack();
                    break;

                case WeaponType.Taser:
                    animator.SetBool("IsBusy", true);
                    animator.SetTrigger("Taser");
                    break;
            }
        }
    }

    private void HandleWeaponSelection()
    {
        // Uses the Input Actions system, consistent with the rest of the
        // project, instead of polling Keyboard.current directly.
        if (InputSystem.actions["SelectWeapon1"].WasPressedThisFrame()) SetEquippedWeapon(WeaponType.Melee);
        if (InputSystem.actions["SelectWeapon2"].WasPressedThisFrame()) SetEquippedWeapon(WeaponType.Ranged);
        if (InputSystem.actions["SelectWeapon3"].WasPressedThisFrame()) SetEquippedWeapon(WeaponType.Taser);
    }

    public void SetEquippedWeapon(WeaponType weapon)
    {
        equippedWeapon = weapon;
    }

    void FixedUpdate()
    {
        if (inputLocked)
        {
            body.linearVelocity = Vector2.zero;
            return;
        }

        float speed = moveSpeed;
        if (isStealthed) speed *= stealthSpeedMultiplier;
        speed *= externalSpeedMultiplier;

        body.linearVelocity = moveInput * speed;
    }
}