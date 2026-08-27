using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Damageable))]
public class EngineerEnemy : MonoBehaviour
{
    private enum State { Idle, Pursue, Hold, Retreat, Dead }

    [SerializeField] private EngineerConfig config;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject turretPrefab;

    [Header("Animation")]
    [SerializeField] private Transform toolVisual;
    [SerializeField] private float flourishAngle = 60f;
    [SerializeField] private float flourishDuration = 0.15f;

    private Rigidbody2D body;
    private Damageable damageable;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;

    private State currentState = State.Idle;
    private Vector2 moveDirection = Vector2.zero;
    private float turretTimer = 0f;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        damageable = GetComponent<Damageable>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        body.gravityScale = 0f;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }

        damageable.OnDeath.AddListener(HandleDeath);
    }

    private void Update()
    {
        if (currentState == State.Dead || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Vector2 directionToPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;

        switch (currentState)
        {
            case State.Idle:
                moveDirection = Vector2.zero;
                if (CanDetectPlayer(distanceToPlayer))
                {
                    currentState = State.Pursue;
                }
                break;

            case State.Pursue:
                if (distanceToPlayer <= config.preferredRange)
                {
                    moveDirection = Vector2.zero;
                    currentState = State.Hold;
                }
                else
                {
                    moveDirection = directionToPlayer;
                }
                break;

            case State.Hold:
                moveDirection = Vector2.zero;

                if (distanceToPlayer < config.retreatRange)
                {
                    currentState = State.Retreat;
                }
                else if (distanceToPlayer > config.preferredRange)
                {
                    currentState = State.Pursue;
                }
                else
                {
                    turretTimer -= Time.deltaTime;
                    if (turretTimer <= 0f)
                    {
                        DeployTurret();
                        turretTimer = config.turretDeployCooldown;
                    }
                }
                break;

            case State.Retreat:
                moveDirection = -directionToPlayer;
                if (distanceToPlayer >= config.preferredRange)
                {
                    currentState = State.Hold;
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
        body.linearVelocity = moveDirection * config.moveSpeed;
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

    private void DeployTurret()
    {
        if (turretPrefab != null)
        {
            Instantiate(turretPrefab, transform.position, Quaternion.identity);
        }

        if (toolVisual != null)
        {
            StartCoroutine(PlayFlourish());
        }
    }

    private IEnumerator PlayFlourish()
    {
        Quaternion start = Quaternion.identity;
        Quaternion peak = Quaternion.Euler(0f, 0f, flourishAngle);

        float elapsed = 0f;
        while (elapsed < flourishDuration)
        {
            elapsed += Time.deltaTime;
            toolVisual.localRotation = Quaternion.Slerp(start, peak, elapsed / flourishDuration);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < flourishDuration)
        {
            elapsed += Time.deltaTime;
            toolVisual.localRotation = Quaternion.Slerp(peak, start, elapsed / flourishDuration);
            yield return null;
        }

        toolVisual.localRotation = start;
    }

    private void HandleDeath()
    {
        currentState = State.Dead;
        moveDirection = Vector2.zero;
        body.linearVelocity = Vector2.zero;
        Destroy(gameObject, config.deathCleanupDelay);
    }
}