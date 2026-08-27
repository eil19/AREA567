using UnityEngine;

[CreateAssetMenu(fileName = "TurretConfig", menuName = "Enemies/Turret Config")]
public class TurretConfig : ScriptableObject
{
    public float detectionRange = 7f;
    public int shotsPerBurst = 3;
    public float burstShotInterval = 0.25f;
    public float burstCooldown = 3f;
    public float projectileSpeed = 7f;
    public float lifetime = 20f;
    public Color rechargingTint = new Color(1f, 1f, 1f, 1f);
}