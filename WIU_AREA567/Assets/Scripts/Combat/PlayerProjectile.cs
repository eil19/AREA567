using UnityEngine;

// Put this on a player-fired projectile prefab. The prefab must have a trigger
// Collider2D and Rigidbody2D; Launch is called immediately after it is spawned.
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerProjectile : MonoBehaviour
{
    [SerializeField] private int damageAmount = 20;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private string targetTag = "Alien";

    private Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction)
    {
        direction = direction.sqrMagnitude > 0f ? direction.normalized : Vector2.down;
        body.linearVelocity = direction * speed;
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // The projectile is spawned at the player's attack point, so never hit its owner.
        if (other.CompareTag("Player")) return;

        Damageable damageable = other.GetComponentInParent<Damageable>();
        if (damageable != null && damageable.CompareTag(targetTag))
        {
            damageable.TakeDamage(damageAmount);
        }

        // A shot is consumed by its target, walls, and any other obstacle.
        Destroy(gameObject);
    }
}
