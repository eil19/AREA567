using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Damageable))]
public class JanitorEnemy : MonoBehaviour
{
    private enum State { Idle, Chase, Attack, Dead }

    [SerializeField] private JanitorConfig config;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject puddlePrefab;

    private Rigidbody2D body;
    private Damageable damageable;
    private Damageable playerDamageable;

    private State currentState = State.Idle;
    private Vector2 moveDirection = Vector2.zero;
    private float attackTimer = 0f;
    private float puddleTimer = 0f;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        damageable = GetComponent<Damageable>();
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

        damageable.OnDeath.AddListener(HandleDeath);
        puddleTimer = config.puddleCooldown;
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
                if (distanceToPlayer <= config.detectionRange)
                {
                    currentState = State.Chase;
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

        if (currentState == State.Chase || currentState == State.Attack)
        {
            puddleTimer -= Time.deltaTime;
            if (puddleTimer <= 0f)
            {
                DropPuddle();
                puddleTimer = config.puddleCooldown;
            }
        }
    }

    private void FixedUpdate()
    {
        if (currentState == State.Dead) return;
        body.linearVelocity = moveDirection * config.moveSpeed;
    }

    private void Attack()
    {
        if (playerDamageable != null)
        {
            playerDamageable.TakeDamage(config.damageAmount);
        }
    }

    private void DropPuddle()
    {
        if (puddlePrefab == null || player == null) return;
        Instantiate(puddlePrefab, player.position, Quaternion.identity);
    }

    private void HandleDeath()
    {
        currentState = State.Dead;
        moveDirection = Vector2.zero;
        body.linearVelocity = Vector2.zero;
        Destroy(gameObject, config.deathCleanupDelay);
    }
}
