using UnityEngine;
using UnityEngine.Events;

public class BossDamageable : Damageable
{
    [Header("Toughness")]
    public int maxToughness = 100;
    [SerializeField] private int currentToughness;
    public int CurrentToughness => currentToughness;
    public bool ToughnessBroken => currentToughness <= 0;

    public UnityEvent onToughnessChanged;
    public UnityEvent onToughnessBroken;

    [Header("Minion Protection")]
    [SerializeField] private int protectingMinionCount = 0;
    public bool IsProtected => protectingMinionCount > 0;

    private void Awake()
    {
        currentToughness = maxToughness;
    }

    // Called by SkeletonMinion when it spawns/dies.
    public void RegisterProtector()
    {
        protectingMinionCount++;
    }

    public void UnregisterProtector()
    {
        protectingMinionCount = Mathf.Max(0, protectingMinionCount - 1);
    }

    // Lets you reset the boss for a retry attempt, same idea as Damageable.ResetHealth().
    public void ResetToughness()
    {
        currentToughness = maxToughness;
        onToughnessChanged?.Invoke();
    }

    public override void TakeDamage(int damage)
    {
        if (IsDead || damage <= 0) return;

        // Fully immune while skeletons are alive.
        if (IsProtected) return;

        if (!ToughnessBroken)
        {
            NotifyDamaged(damage); // still triggers Hurt animation even though health isn't affected

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
}