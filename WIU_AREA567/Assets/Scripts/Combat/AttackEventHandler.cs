using System.Collections;
using UnityEngine;

// Attach to the Player. Reads its AttackPoint directly from PlayerController
// (single source of truth - no need to separately drag the same child
// transform into two different fields).
// Wire AttackCheck() to a keyframe on the Attack animation clip and
// AttackEnd() to a later keyframe via Animation Events.
// TaserCheck() works the same way, wired to a Taser animation clip instead.

[RequireComponent(typeof(PlayerController))]
public class AttackEventHandler : MonoBehaviour
{
    private Transform attackPoint;
    private Animator animator;

    [SerializeField] private LayerMask layerToCheck;
    [SerializeField] private float attackRadius = 0.2f;
    [SerializeField] private int damageAmount = 10;

    [Header("Taser")]
    [Tooltip("GameObject tag used to identify Aliens - Taser only affects objects with this tag.")]
    [SerializeField] private string alienTag = "Alien";
    [SerializeField] private float stunDuration = 3f;

    void Awake()
    {
        attackPoint = GetComponent<PlayerController>().AttackPoint;
        animator = GetComponent<Animator>();
        if (attackPoint == null)
        {
            Debug.LogWarning($"{gameObject.name}: AttackEventHandler couldn't find an Attack Point - assign one on PlayerController's Inspector field.");
        }
    }

    [ContextMenu("Test Taser (Editor Only)")]
    private void DebugTestTaser()
    {
        TaserCheck();
    }

    public void AttackCheck()
    {
        if (attackPoint == null) return;

        // Toggling active here is currently cosmetic-only (OverlapCircle below
        // doesn't need it) - kept in case a weapon-swing visual gets attached
        // to attackPoint later that should only be visible during the attack window.
        attackPoint.gameObject.SetActive(true);
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRadius, layerToCheck);
        if (hit != null && hit.TryGetComponent(out Damageable damagedObject))
        {
            damagedObject.TakeDamage(damageAmount);
        }
    }

    // Wire to a keyframe on your Taser animation clip via Animation Events,
    // same pattern as AttackCheck(). Only affects objects tagged "Alien" -
    // stuns instead of damaging, does nothing to non-alien targets.
    public void TaserCheck()
    {
        if (attackPoint == null) return;

        attackPoint.gameObject.SetActive(true);
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRadius, layerToCheck);
        if (hit != null && hit.CompareTag(alienTag) && hit.TryGetComponent(out Stunnable stunnableTarget))
        {
            stunnableTarget.Stun(stunDuration);
        }
    }

    public void AttackEnd()
    {
        if (attackPoint == null) return;
        attackPoint.gameObject.SetActive(false);
        if (animator != null) animator.SetBool("IsBusy", false);
    }

    // Called by AttackBoostItemEffect-style buffs, if you build one for this project.
    public void ApplyDamageBoost(float multiplier, float duration)
    {
        StartCoroutine(BoostRoutine(multiplier, duration));
    }

    private IEnumerator BoostRoutine(float multiplier, float duration)
    {
        int original = damageAmount;
        damageAmount = Mathf.RoundToInt(damageAmount * multiplier);
        yield return new WaitForSeconds(duration);
        damageAmount = original;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}