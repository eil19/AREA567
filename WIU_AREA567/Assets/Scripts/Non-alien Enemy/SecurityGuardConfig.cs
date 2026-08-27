using UnityEngine;

[CreateAssetMenu(fileName = "SecurityGuardConfig", menuName = "Enemies/Security Guard Config")]
public class SecurityGuardConfig : ScriptableObject
{
    public float moveSpeed = 4.5f;
    public float detectionRange = 8f;
    public float meleeRange = 1.2f;
    public float attackCooldown = 1f;
    public int damageAmount = 15;
    public float deathCleanupDelay = 1f;

    [Header("Detection")]
    public float stealthDetectionMultiplier = 0.5f;
    public LayerMask wallLayer;
    public float giveUpRange = 14f;

    [Header("Patrol")]
    public float patrolSpeed = 2f;
    public float waypointReachDistance = 0.2f;
}