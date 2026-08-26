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

    private Rigidbody2D body;
    private Damageable damageable;

    private State currentState = State.Idle;
    private Vector2 moveDirection = Vector2.zero;
    private float throwTimer = 0f;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        damageable = GetComponent<Damageable>();
        body.gravityScale = 0f;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
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
                if (distanceToPlayer <= config.detectionRange)
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
                else
                {
                    throwTimer -= Time.deltaTime;
                    if (throwTimer <= 0f)
                    {
                        ThrowFlask(directionToPlayer);
                        throwTimer = config.throwCooldown;
                    }
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
    }

    private void FixedUpdate()
    {
        if (currentState == State.Dead) return;
        body.linearVelocity = moveDirection * config.moveSpeed;
    }

    private void ThrowFlask(Vector2 direction)
    {
        if (projectilePrefab == null) return;

        Vector3 spawnPos = projectileSpawnPoint != null ? projectileSpawnPoint.position : transform.position;
        GameObject flask = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        if (flask.TryGetComponent(out Rigidbody2D flaskBody))
        {
            flaskBody.linearVelocity = direction * config.projectileSpeed;
        }
    }

    private void HandleDeath()
    {
        currentState = State.Dead;
        moveDirection = Vector2.zero;
        body.linearVelocity = Vector2.zero;
        Destroy(gameObject, config.deathCleanupDelay);
    }
}
