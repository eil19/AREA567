using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Damageable))]
public class ScientistEnemy : MonoBehaviour
{
    private enum State { Idle, Pursue, Attack, Retreat, Dead }

    [SerializeField] private ScientistEnemyConfig config;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;

    [Header("Animation")]
    [SerializeField] private Transform throwItemVisual;
    [SerializeField] private float throwRotationAngle = 60f;
    [SerializeField] private float throwRotationDuration = 0.15f;

    private Rigidbody2D body;
    private Damageable damageable;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;

    private State currentState = State.Idle;
    private Vector2 moveDirection = Vector2.zero;
    private float throwTimer = 0f;

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
                    currentState = State.Attack;
                }
                else
                {
                    moveDirection = directionToPlayer;
                }
                break;

            case State.Attack:
                moveDirection = Vector2.zero;

                if (distanceToPlayer < config.retreatRange)
                {
                    currentState = State.Retreat;
                }
                else if (distanceToPlayer > config.preferredRange)
                {
                    currentState = State.Pursue;
                }
                break;

            case State.Retreat:
                moveDirection = -directionToPlayer;
                if (distanceToPlayer >= config.preferredRange)
                {
                    currentState = State.Attack;
                }
                break;
        }

        if (currentState == State.Attack || currentState == State.Retreat)
        {
            throwTimer -= Time.deltaTime;
            if (throwTimer <= 0f)
            {
                ThrowFlask(directionToPlayer);
                throwTimer = config.throwCooldown;
            }
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

    private void ThrowFlask(Vector2 direction)
    {
        if (projectilePrefab != null)
        {
            Vector3 spawnPos = projectileSpawnPoint != null ? projectileSpawnPoint.position : transform.position;
            GameObject flask = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

            if (flask.TryGetComponent(out Rigidbody2D flaskBody))
            {
                flaskBody.linearVelocity = direction * config.projectileSpeed;
            }
        }

        if (throwItemVisual != null)
        {
            StartCoroutine(PlayThrowRotation());
        }
    }

    private IEnumerator PlayThrowRotation()
    {
        Quaternion start = Quaternion.identity;
        Quaternion peak = Quaternion.Euler(0f, 0f, -throwRotationAngle);

        float elapsed = 0f;
        while (elapsed < throwRotationDuration)
        {
            elapsed += Time.deltaTime;
            throwItemVisual.localRotation = Quaternion.Slerp(start, peak, elapsed / throwRotationDuration);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < throwRotationDuration)
        {
            elapsed += Time.deltaTime;
            throwItemVisual.localRotation = Quaternion.Slerp(peak, start, elapsed / throwRotationDuration);
            yield return null;
        }

        throwItemVisual.localRotation = start;
    }

    private void HandleDeath()
    {
        currentState = State.Dead;
        moveDirection = Vector2.zero;
        body.linearVelocity = Vector2.zero;
        Destroy(gameObject, config.deathCleanupDelay);
    }
}