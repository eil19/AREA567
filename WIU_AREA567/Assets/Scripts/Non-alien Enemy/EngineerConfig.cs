using UnityEngine;

[CreateAssetMenu(fileName = "EngineerConfig", menuName = "Enemies/Engineer Config")]
public class EngineerConfig : ScriptableObject
{
    public float moveSpeed = 3f;
    public float detectionRange = 8f;
    public float preferredRange = 6f;
    public float retreatRange = 3f;
    public float turretDeployCooldown = 4f;
    public float deathCleanupDelay = 1f;

    [Header("Detection")]
    public float stealthDetectionMultiplier = 0.5f;
    public LayerMask wallLayer;
    public float giveUpRange = 14f;

    [Header("Robot Variety")]
    public float spawnRadius = 1.2f;
}