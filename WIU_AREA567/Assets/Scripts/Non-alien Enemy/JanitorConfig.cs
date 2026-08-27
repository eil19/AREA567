using UnityEngine;

[CreateAssetMenu(fileName = "JanitorConfig", menuName = "Enemies/Janitor Config")]
public class JanitorConfig : ScriptableObject
{
    public float moveSpeed = 3.5f;
    public float detectionRange = 8f;
    public float meleeRange = 1.2f;
    public float attackCooldown = 1f;
    public int damageAmount = 12;
    public float puddleCooldown = 3f;
    public float deathCleanupDelay = 1f;

    [Header("Detection")]
    public float stealthDetectionMultiplier = 0.5f;
    public LayerMask wallLayer;
    public float giveUpRange = 14f;
}