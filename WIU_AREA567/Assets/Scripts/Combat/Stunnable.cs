using UnityEngine;
using UnityEngine.Events;
using System.Collections;

// Kept separate from Damageable since Player and non-alien enemies never
// need this; only add it to Alien prefabs.
public class Stunnable : MonoBehaviour
{
    private bool isStunned;
    public bool IsStunned => isStunned;

    [Header("Events")]
    public UnityEvent OnStunStart; // hook up stun VFX/animation later
    public UnityEvent OnStunEnd;

    private Coroutine stunRoutine;

    // Called by AttackEventHandler.TaserCheck() on a successful hit.
    public void Stun(float duration)
    {
        if (stunRoutine != null)
        {
            StopCoroutine(stunRoutine);
        }
        stunRoutine = StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        if (!isStunned)
        {
            isStunned = true;
            Debug.Log($"{gameObject.name} is now stunned for {duration}s"); // TODO: remove once stun VFX exists
            OnStunStart?.Invoke();
        }

        yield return new WaitForSeconds(duration);

        isStunned = false;
        stunRoutine = null;
        Debug.Log($"{gameObject.name} stun ended"); // TODO: remove once stun VFX exists
        OnStunEnd?.Invoke();
    }
}