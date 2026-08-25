using UnityEngine;

[CreateAssetMenu(fileName = "TurretConfig", menuName = "Enemies/Turret Config")]
public class TurretConfig : ScriptableObject
{
    public float detectionRange = 7f;
    public float fireCooldown = 1.2f;
    public float projectileSpeed = 7f;
    public float lifetime = 20f;
}
