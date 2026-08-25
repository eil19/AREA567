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
}
