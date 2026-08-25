using UnityEngine;

[CreateAssetMenu(fileName = "ScientistEnemyConfig", menuName = "Enemies/Scientist Config")]
public class ScientistEnemyConfig : ScriptableObject
{
    public float moveSpeed = 3f;
    public float detectionRange = 8f;
    public float preferredRange = 5f;
    public float retreatRange = 2.5f;
    public float throwCooldown = 1.5f;
    public float projectileSpeed = 6f;
    public float deathCleanupDelay = 1f;
}
