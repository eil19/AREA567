using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Damageable))]
public class Turret : MonoBehaviour
{
    [SerializeField] private TurretConfig config;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;

    private Damageable damageable;
    private SpriteRenderer spriteRenderer;
    private Color normalColor;

    private float timer = 0f;
    private int shotsFiredInBurst = 0;
    private bool isRecharging = false;
    private bool isDead = false;

    private void Awake()
    {
        damageable = GetComponent<Damageable>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) normalColor = spriteRenderer.color;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        damageable.OnDeath.AddListener(HandleDeath);
    }

    private void Start()
    {
        Destroy(gameObject, config.lifetime);
    }

    private void Update()
    {
        if (isDead || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > config.detectionRange) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        if (isRecharging)
        {
            isRecharging = false;
            shotsFiredInBurst = 0;
            SetRecharging(false);
        }
        else
        {
            Fire();
            shotsFiredInBurst++;

            if (shotsFiredInBurst >= config.shotsPerBurst)
            {
                isRecharging = true;
                timer = config.burstCooldown;
                SetRecharging(true);
            }
            else
            {
                timer = config.burstShotInterval;
            }
        }
    }

    private void SetRecharging(bool recharging)
    {
        if (spriteRenderer == null) return;
        spriteRenderer.color = recharging ? config.rechargingTint : normalColor;
    }

    private void Fire()
    {
        if (projectilePrefab == null || player == null) return;

        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        Vector3 spawnPos = projectileSpawnPoint != null ? projectileSpawnPoint.position : transform.position;
        GameObject bolt = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        if (bolt.TryGetComponent(out Rigidbody2D boltBody))
        {
            boltBody.linearVelocity = direction * config.projectileSpeed;
        }
    }

    private void HandleDeath()
    {
        isDead = true;
        Destroy(gameObject);
    }
}