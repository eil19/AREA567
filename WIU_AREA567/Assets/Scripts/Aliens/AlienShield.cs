using UnityEngine;
using UnityEngine.Events;
public class AlienShield : MonoBehaviour
{
    [Header("Shield")]
    [SerializeField] private int maxShieldHealth = 50;
    private int currentShieldHealth;

    [Header("Regeneration")]
    [Tooltip("Seconds after the shield breaks before AlienTankAction is allowed to raise it again.")]
    [SerializeField] private float regenDelay = 5f;
    private float brokenAtTime = -999f;

    [Header("Events")]
    public UnityEvent<int, int> OnShieldChanged; // (current, max) - hook to a shield bar/VFX
    public UnityEvent OnShieldBroken;
    public UnityEvent OnShieldRestored;

    public bool HasShield => currentShieldHealth > 0;
    public int CurrentShieldHealth => currentShieldHealth;
    public int MaxShieldHealth => maxShieldHealth;

    void Awake()
    {
        currentShieldHealth = maxShieldHealth;
    }

    // Called by Damageable.TakeDamage() before health is touched.
    // Returns whatever damage is left over after the shield eats what it can.
    public int AbsorbDamage(int incomingDamage)
    {
        if (currentShieldHealth <= 0) return incomingDamage;

        int absorbed = Mathf.Min(currentShieldHealth, incomingDamage);
        currentShieldHealth -= absorbed;
        OnShieldChanged?.Invoke(currentShieldHealth, maxShieldHealth);

        if (currentShieldHealth <= 0)
        {
            brokenAtTime = Time.time;
            OnShieldBroken?.Invoke();
        }

        return incomingDamage - absorbed;
    }

    public void TryRegenerate()
    {
        if (HasShield) return;
        if (Time.time - brokenAtTime < regenDelay) return;

        currentShieldHealth = maxShieldHealth;
        OnShieldChanged?.Invoke(currentShieldHealth, maxShieldHealth);
        OnShieldRestored?.Invoke();
    }
}