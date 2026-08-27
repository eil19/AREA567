using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Damageable))]
public class SecurityGuardEnemy : MonoBehaviour
{
    private enum State { Idle, Chase, Attack, Dead }

    [SerializeField] private SecurityGuardConfig config;
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] patrolPoints;

    [Header("Animation")]
    [SerializeField] private Transform batonVisual;
    [SerializeField] private float swingAngle = 70f;
    [SerializeField] private float swingDuration = 0.12f;

    [Header("Damage Feedback")]
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.15f;

    private Rigidbody2D body;
    private Damageable damageable;
    private Damageable playerDamageable;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;

    private State currentState = State.Idle;
    private Vector2 moveDirection = Vector2.zero;
    private float attackTimer = 0f;
    private int currentWaypointIndex = 0;
    private Color normalColor;
    private Coroutine flashRoutine;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        damageable = GetComponent<Damageable>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) normalColor = spriteRenderer.color;
        body.gravityScale = 0f;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerDamageable = playerObj.GetComponent<Damageable>();
            }
        }
        else
        {
            playerDamageable = player.GetComponent<Damageable>();
        }

        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }

        damageable.OnDeath.AddListener(HandleDeath);
        damageable.OnDamaged.AddListener(HandleDamaged);
    }

    private void Update()
    {
        if (currentState == State.Dead || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Vector2 directionToPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;

        if (currentState != State.Idle && distanceToPlayer > config.giveUpRange)
        {
            currentState = State.Idle;
            moveDirection = Vector2.zero;
        }

        switch (currentState)
        {
            case State.Idle:
                if (CanDetectPlayer(distanceToPlayer))
                {
                    currentState = State.Chase;
                    moveDirection = Vector2.zero;
                }
                else
                {
                    UpdatePatrol();
                }
                break;

            case State.Chase:
                if (distanceToPlayer <= config.meleeRange)
                {
                    moveDirection = Vector2.zero;
                    currentState = State.Attack;
                }
                else
                {
                    moveDirection = directionToPlayer;
                }
                break;

            case State.Attack:
                moveDirection = Vector2.zero;

                if (distanceToPlayer > config.meleeRange)
                {
                    currentState = State.Chase;
                }
                else
                {
                    attackTimer -= Time.deltaTime;
                    if (attackTimer <= 0f)
                    {
                        Attack();
                        attackTimer = config.attackCooldown;
                    }
                }
                break;
        }

        Vector2 facingDirection = moveDirection.sqrMagnitude > 0.01f ? moveDirection : directionToPlayer;
        bool isMoving = moveDirection.sqrMagnitude > 0.01f;
        UpdateAnimator(facingDirection, isMoving);
    }

    private void FixedUpdate()
    {
        if (currentState == State.Dead) return;
        float speed = currentState == State.Idle ? config.patrolSpeed : config.moveSpeed;
        body.linearVelocity = moveDirection * speed;
    }

    private void UpdatePatrol()
    {
        if (patrolPoints == null ||
            patrolPoints.Length == 0)
        {
            moveDirection = Vector2.zero;
            return;
        }

        if (currentWaypointIndex < 0 ||
            currentWaypointIndex >= patrolPoints.Length)
        {
            currentWaypointIndex = 0;
        }

        Transform target =
            patrolPoints[currentWaypointIndex];

        // Protect against an unassigned waypoint.
        if (target == null)
        {
            moveDirection = Vector2.zero;

            currentWaypointIndex =
                (currentWaypointIndex + 1) %
                patrolPoints.Length;

            return;
        }

        Vector2 toTarget =
            (Vector2)target.position -
            (Vector2)transform.position;

        if (toTarget.magnitude <=
            config.waypointReachDistance)
        {
            currentWaypointIndex =
                (currentWaypointIndex + 1) %
                patrolPoints.Length;

            moveDirection = Vector2.zero;
        }
        else
        {
            moveDirection =
                toTarget.normalized;
        }
    }

    private bool CanDetectPlayer(float distanceToPlayer)
    {
        float effectiveRange = config.detectionRange;
        if (playerController != null && playerController.IsStealthed)
        {
            effectiveRange *= config.stealthDetectionMultiplier;
        }

        if (distanceToPlayer > effectiveRange) return false;

        RaycastHit2D hit = Physics2D.Linecast(transform.position, player.position, config.wallLayer);
        return hit.collider == null;
    }

    private void UpdateAnimator(Vector2 facingDirection, bool isMoving)
    {
        if (animator == null) return;

        animator.speed = isMoving ? 1f : 0f;
        animator.SetBool("IsMoving", isMoving);

        if (facingDirection.sqrMagnitude < 0.0001f) return;

        animator.SetFloat("MoveX", facingDirection.x);
        animator.SetFloat("MoveY", facingDirection.y);

        int directionIndex;
        if (Mathf.Abs(facingDirection.y) >= Mathf.Abs(facingDirection.x))
        {
            directionIndex = facingDirection.y > 0 ? 1 : 0;
        }
        else
        {
            directionIndex = 2;
            if (spriteRenderer != null) spriteRenderer.flipX = facingDirection.x > 0;
        }

        animator.SetInteger("Direction", directionIndex);
    }

    private void Attack()
    {
        if (playerDamageable != null)
        {
            playerDamageable.TakeDamage(config.damageAmount);
        }

        if (batonVisual != null)
        {
            StartCoroutine(PlaySwing());
        }
    }

    private IEnumerator PlaySwing()
    {
        Quaternion start = Quaternion.identity;
        Quaternion peak = Quaternion.Euler(0f, 0f, swingAngle);

        float elapsed = 0f;
        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            batonVisual.localRotation = Quaternion.Slerp(start, peak, elapsed / swingDuration);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            batonVisual.localRotation = Quaternion.Slerp(peak, start, elapsed / swingDuration);
            yield return null;
        }

        batonVisual.localRotation = start;
    }

    private void HandleDamaged(int amount)
    {
        if (spriteRenderer == null) return;

        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRed());
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = normalColor;
        flashRoutine = null;
    }

    private void HandleDeath()
    {
        currentState = State.Dead;
        moveDirection = Vector2.zero;
        body.linearVelocity = Vector2.zero;
        Destroy(gameObject, config.deathCleanupDelay);
    }
}