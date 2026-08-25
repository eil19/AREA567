using UnityEngine;
public class AlienFireball : MonoBehaviour
{
    private int damage;
    private LayerMask hitLayer;

    public void Launch(Vector2 direction, float speed, int damage, LayerMask hitLayer, float lifetime = 4f)
    {
        this.damage = damage;
        this.hitLayer = hitLayer;

        if (TryGetComponent(out Rigidbody2D body))
        {
            body.gravityScale = 0f;
            body.linearVelocity = direction.normalized * speed;
        }

        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & hitLayer) == 0) return;

        if (other.TryGetComponent(out Damageable damageable))
        {
            damageable.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}