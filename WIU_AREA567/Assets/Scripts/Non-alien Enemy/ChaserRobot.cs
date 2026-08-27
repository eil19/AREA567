using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Damageable))]
public class ChaserRobot : MonoBehaviour
{
    private enum State { Idle, Chase, Dead }

    [SerializeField] private ChaserRobotConfig config;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject explosionVFXPrefab;

    private Rigidbody2D body;
    private Damageable damageable;
    private Damageable playerDamageable;
    private PlayerController playerController;

    private State currentState = State.Idle;
    private Vector2 moveDirection = Vector2.zero;

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

        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }

        damageable.OnDeath.AddListener(HandleDeath);
    }

    private void Start()
    {
        Destroy(gameObject, config.lifetime);
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
                moveDirection = Vector2.zero;
                if (CanDetectPlayer(distanceToPlayer))
                {
                    currentState = State.Chase;
                }
                break;

            case State.Chase:
                if (distanceToPlayer <= config.explodeRange)
                {
                    Explode();
                }
                else
                {
                    moveDirection = directionToPlayer;
                }
                break;
        }
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

    private void Explode()
    {
        if (playerDamageable != null)
        {
            playerDamageable.TakeDamage(config.explosionDamage);
        }

        if (explosionVFXPrefab != null)
        {
            Instantiate(explosionVFXPrefab, transform.position, Quaternion.identity);
        }

        currentState = State.Dead;
        moveDirection = Vector2.zero;
        body.linearVelocity = Vector2.zero;
        Destroy(gameObject);
    }

    private void HandleDeath()
    {
        currentState = State.Dead;
        moveDirection = Vector2.zero;
        body.linearVelocity = Vector2.zero;
        Destroy(gameObject);
    }
}
