using UnityEngine;
using UnityEngine.Events;

public class BossDamageable : Damageable
{
    [Header("Phases")]
    [SerializeField] private int phase = 1;
    public int Phase => phase;
    public UnityEvent onPhaseTwoEntered; // wire animation/roar/camera shake here

    [Header("Toughness")]
    public int maxToughness = 100;
    [SerializeField] private int currentToughness;
    public int CurrentToughness => currentToughness;
    public bool ToughnessBroken => currentToughness <= 0;

    public UnityEvent onToughnessChanged; // wire to ToughnessBar
    public UnityEvent onToughnessBroken;

    [Header("Minion Protection")]
    [SerializeField] private int protectingMinionCount = 0;
    public bool IsProtected => protectingMinionCount > 0;

    [HideInInspector] public float lastMinionDeathTime = -999f;

    protected override void Awake()
    {
        base.Awake(); // this is what was missing - health was never being reset to maxHealth
        currentToughness = maxToughness;
    }

    public void RegisterProtector()
    {
        protectingMinionCount++;
    }

    public void UnregisterProtector()
    {
        protectingMinionCount = Mathf.Max(0, protectingMinionCount - 1);
        if (protectingMinionCount == 0)
        {
            lastMinionDeathTime = Time.time;
        }
    }

    public void ResetToughness()
    {
        currentToughness = maxToughness;
        onToughnessChanged?.Invoke();
    }

    public override void TakeDamage(int damage)
    {
        if (CurrentHealth <= 0 || damage <= 0) return;

        // Fully immune while protectors are alive.
        if (IsProtected) return;

        if (!ToughnessBroken)
        {
            OnDamaged?.Invoke(damage); // still play the Hit reaction even though health isn't touched yet

            currentToughness = Mathf.Max(0, currentToughness - damage);
            onToughnessChanged?.Invoke();

            if (ToughnessBroken)
            {
                onToughnessBroken?.Invoke();
            }
            return;
        }

        base.TakeDamage(damage);
    }

    // Called by base.TakeDamage() the instant health would hit zero before OnDeath fires.
    protected override void HandleDeath()
    {
        if (phase == 1)
        {
            phase = 2;
            Heal(MaxHealth); // refill for the second life bar
            ResetToughness();
            onPhaseTwoEntered?.Invoke();
            return; 
        }

        base.HandleDeath(); // phase 2 -> real death, VFX + OnDeath fire normally
    }
}