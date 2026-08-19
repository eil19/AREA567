using System.Collections;
using UnityEngine;

// Attach to the Player. Assign a child "AttackPoint" transform (in the new
// top-down project, PlayerController repositions this automatically to
// face FacingDirection every frame - no manual flipping needed).
// Wire AttackCheck() to a keyframe on the Attack animation clip and
// AttackEnd() to a later keyframe via Animation Events.
// TaserCheck() works the same way, wired to a Taser animation clip instead.

public class AttackEventHandler : MonoBehaviour
{
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask layerToCheck;
    [SerializeField] private float attackRadius = 0.2f;
    [SerializeField] private int damageAmount = 10;

    [Header("Taser")]
    [Tooltip("GameObject tag used to identify Aliens - Taser only affects objects with this tag.")]
    [SerializeField] private string alienTag = "Alien";
    [SerializeField] private float stunDuration = 3f;

    public void AttackCheck()
    {
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
        attackPoint.gameObject.SetActive(true);
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRadius, layerToCheck);
        if (hit != null && hit.CompareTag(alienTag) && hit.TryGetComponent(out Stunnable stunnableTarget))
        {
            stunnableTarget.Stun(stunDuration);
        }
    }

    public void AttackEnd()
    {
        attackPoint.gameObject.SetActive(false);
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
