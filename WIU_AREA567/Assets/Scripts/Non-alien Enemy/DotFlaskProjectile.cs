using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DotFlaskProjectile : MonoBehaviour
{
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private int damagePerTick = 5;
    [SerializeField] private float tickInterval = 1f;
    [SerializeField] private float duration = 4f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(targetTag)) return;

        DamageOverTimeEffect existing = other.GetComponent<DamageOverTimeEffect>();
        if (existing != null)
        {
            existing.Refresh(damagePerTick, tickInterval, duration);
        }
        else
        {
            DamageOverTimeEffect effect = other.gameObject.AddComponent<DamageOverTimeEffect>();
            effect.Initialize(damagePerTick, tickInterval, duration);
        }

        Destroy(gameObject);
    }
}
