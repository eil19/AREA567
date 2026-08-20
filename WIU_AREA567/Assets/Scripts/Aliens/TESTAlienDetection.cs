using UnityEngine;

public class TESTAlienDetection : MonoBehaviour
{
    public float followDistance = 3f;
    public float enemyDetectRadius = 5f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyDetectRadius);
    }
}
