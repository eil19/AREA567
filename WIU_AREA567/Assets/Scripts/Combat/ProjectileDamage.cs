using UnityEngine;

// Attach to any projectile prefab (Scientist's chemical attack, Flying
// alien's fire attack, etc). Projectile's velocity/movement is set by
// whatever spawns it - this script only handles what happens on hit.
[RequireComponent(typeof(Collider2D))]
public class ProjectileDamage : MonoBehaviour
{
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private float lifetime = 5f; // auto-destroy if it never hits anything

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(targetTag)) return;

        if (other.TryGetComponent(out Damageable damageable))
        {
            damageable.TakeDamage(damageAmount);
        }

        Destroy(gameObject);
    }
}
