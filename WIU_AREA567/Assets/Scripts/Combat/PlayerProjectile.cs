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
    [Tooltip("Set to the Enemy layer - covers both alien and non-alien enemies, same layer AttackEventHandler's melee check uses.")]
    [SerializeField] private LayerMask targetLayer;

    private Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction)
    {
        direction = direction.sqrMagnitude > 0f ? direction.normalized : Vector2.down;
        body.linearVelocity = direction * speed;

        // Rotate the sprite to visually face the direction of travel.
        // The "- 90f" assumes the bullet art is drawn pointing UP by default -
        // see note below if it looks wrong.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // The projectile is spawned at the player's attack point, so never hit its owner.
        if (other.CompareTag("Player")) return;

        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            Damageable damageable = other.GetComponentInParent<Damageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damageAmount);
            }
        }

        // A shot is consumed by its target, walls, and any other obstacle -
        // this still happens even if the thing hit wasn't on targetLayer,
        // same "consumed by anything it touches" behavior as before.
        Destroy(gameObject);
    }
}