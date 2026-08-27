using System.Collections;
using UnityEngine;

public class DamageOverTimeEffect : MonoBehaviour
{
    private int damagePerTick;
    private float tickInterval;
    private float remainingDuration;
    private Damageable damageable;
    private Coroutine tickRoutine;

    public void Initialize(int damage, float interval, float duration)
    {
        damagePerTick = damage;
        tickInterval = interval;
        remainingDuration = duration;
        damageable = GetComponent<Damageable>();

        if (tickRoutine == null)
        {
            tickRoutine = StartCoroutine(TickDamage());
        }
    }

    public void Refresh(int damage, float interval, float duration)
    {
        damagePerTick = damage;
        tickInterval = interval;
        remainingDuration = duration;
    }

    private IEnumerator TickDamage()
    {
        while (remainingDuration > 0f)
        {
            yield return new WaitForSeconds(tickInterval);
            remainingDuration -= tickInterval;

            if (damageable != null)
            {
                damageable.TakeDamage(damagePerTick);
            }
        }

        Destroy(this);
    }
}
