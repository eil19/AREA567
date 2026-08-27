using UnityEngine;

[CreateAssetMenu(fileName = "ChaserRobotConfig", menuName = "Enemies/Chaser Robot Config")]
public class ChaserRobotConfig : ScriptableObject
{
    public float moveSpeed = 5.5f;
    public float detectionRange = 8f;
    public float explodeRange = 0.6f;
    public int explosionDamage = 30;
    public float giveUpRange = 14f;
    public float stealthDetectionMultiplier = 0.5f;
    public LayerMask wallLayer;
    public float lifetime = 15f;
}
