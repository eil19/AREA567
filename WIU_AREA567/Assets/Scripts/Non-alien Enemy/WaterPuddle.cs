using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WaterPuddle : MonoBehaviour
{
    [SerializeField] private float slowMultiplier = 0.5f;
    [SerializeField] private float duration = 4f;
    [SerializeField] private string targetTag = "Player";

    private void Start()
    {
        Destroy(gameObject, duration);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(targetTag)) return;

        if (other.TryGetComponent(out PlayerController playerController))
        {
            playerController.SetSlowMultiplier(slowMultiplier);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(targetTag)) return;

        if (other.TryGetComponent(out PlayerController playerController))
        {
            playerController.ClearSlowMultiplier();
        }
    }
}
