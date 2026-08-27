using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class VisionFlaskProjectile : MonoBehaviour
{
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float impairDuration = 1.5f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(targetTag)) return;

        if (VisionImpairEffect.Instance != null)
        {
            VisionImpairEffect.Instance.TriggerImpairment(impairDuration);
        }

        Destroy(gameObject);
    }
}
